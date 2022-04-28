// See https://aka.ms/new-console-template for more information

using System.Drawing;

Console.WriteLine("Trykk på en knapp på tastaturet ditt for å lage labyrint på skjermen din...");
Console.ReadLine();

var labyrint = new Labyrint(10, 10);
labyrint.Tegn();


class Labyrint
{
    private List<List<Celle>> _celler;
    private List<(Point p1, Point p2)> _kanter;
    private Point _startKoordinat;
    private Point _målKoordinat;
    private int _bredde;
    private int _høyde;

    public Labyrint(int bredde, int høyde)
    {
        _bredde = bredde;
        _høyde = høyde;
        _celler = new List<List<Celle>>();
        _kanter = new List<(Point p1, Point p2)>();
        for (var y = 0; y < høyde; y++)
        {
            var rad = new List<Celle>();
            for (var x = 0; x < bredde; x++)
            {
                rad.Add(new Celle(new Point(x, y)));
            }
            
            _celler.Add(rad);
        }

        _startKoordinat = new Point(0, 0);
        _målKoordinat = new Point(bredde - 1, høyde - 1);
        GenererStier();
    }

    private void GenererStier()
    {
        LagLøsningsSti();
        LagAndreStier();
    }

    private void LagLøsningsSti()
    {
        LetEtterMål(_startKoordinat);
    }

    private bool LetEtterAnnenSti(Point currentKoordinat, List<Point> besøkteKoordinater)
    {
        var retningerÅPrøve = TilfeldigeRetninger();
        foreach (var retning in retningerÅPrøve)
        {
            var nesteKoordinat = new Point(currentKoordinat.X + retning.X, currentKoordinat.Y + retning.Y);
            if (ErUtenforLabyrinten(nesteKoordinat))
            {
                continue;
            }

            if (besøkteKoordinater.Contains(nesteKoordinat))
            {
                continue;
            }
            if (ErDelAvSti(nesteKoordinat))
            {
                LagKant(currentKoordinat, nesteKoordinat);
                return true;
            }
            LagKant(currentKoordinat, nesteKoordinat);
            besøkteKoordinater.Add(nesteKoordinat);
            if (LetEtterAnnenSti(nesteKoordinat, besøkteKoordinater))
            {
                return true;
            }
            else
            {
                FjernKant(currentKoordinat, nesteKoordinat);
                besøkteKoordinater.Remove(nesteKoordinat);
            }
        }

        return false;
    }

    private bool LetEtterMål(Point currentKoordinat)
    {
        var retningerÅPrøve = TilfeldigeRetninger();
        foreach (var retning in retningerÅPrøve)
        {
            var nesteKoordinat = new Point(currentKoordinat.X + retning.X, currentKoordinat.Y + retning.Y);
            if (ErUtenforLabyrinten(nesteKoordinat))
            {
                continue;
            }

            if (ErDelAvSti(nesteKoordinat))
            {
                continue;
            }

            if (nesteKoordinat == _målKoordinat)
            {
                LagKant(currentKoordinat, nesteKoordinat);
                return true;
            }

            LagKant(currentKoordinat, nesteKoordinat);
            if (LetEtterMål(nesteKoordinat))
            {
                return true;
            }
            else
            {
                FjernKant(currentKoordinat, nesteKoordinat);
            }
        }

        return false;
    }

    private bool ErDelAvSti(Point nesteKoordinat)
    {
        foreach (var (p1, p2) in _kanter)
        {
            if (p1 == nesteKoordinat || p2 == nesteKoordinat)
            {
                return true;
            }
        }

        return false;
    }

    private bool ErUtenforLabyrinten(Point p)
    {
        return p.X < 0 || p.X >= _bredde || p.Y < 0 || p.Y >= _høyde;
    }

    private void LagAndreStier()
    {
        while (true)
        {
            var ikkeTilknyttetKoordinat = FinnCelleKoordinatSomIkkeErTilknyttet();
            if (ikkeTilknyttetKoordinat == null)
            {
                break;
            }

            LetEtterAnnenSti(ikkeTilknyttetKoordinat.Value, new List<Point>{ikkeTilknyttetKoordinat.Value});
        }
    }

    private Point? FinnCelleKoordinatSomIkkeErTilknyttet()
    {
        var aktuelleCeller = new List<Point>();
        foreach (var rader in _celler)
        {
            foreach (var celle in rader)
            {
                if (!ErDelAvSti(celle.Koordinat))
                {
                    aktuelleCeller.Add(celle.Koordinat);
                }
            }
        }

        if (aktuelleCeller.Count == 0)
        {
            return null;
        }

        return aktuelleCeller.OrderBy(x => new Random().Next()).First();
    }

    private List<Point> TilfeldigeRetninger()
    {
        var liste = new List<Point>
        {
            new Point(1, 0), //høyre
            new Point(0, 1), //ned
            new Point(-1, 0), //venstre
            new Point(0, -1), //opp
        };
        return liste.OrderBy(a => new Random().Next()).ToList();
    }

    public Celle HentCelle(Point koordinat)
    {
        return _celler[koordinat.Y][koordinat.X];
    }

    public void LagKant(Point celle1Koordinat, Point celle2Koordinat)
    {
        _kanter.Add((celle1Koordinat, celle2Koordinat));
    }

    public void FjernKant(Point celle1Koordinat, Point celle2Koordinat)
    {
        _kanter.Remove((celle1Koordinat, celle2Koordinat));
    }

    public bool ErTilknyttedeCeller(Point celle1Koordinat, Point celle2Koordinat)
    {
        if (_kanter.Contains((celle1Koordinat, celle2Koordinat)) || _kanter.Contains((celle2Koordinat, celle1Koordinat)))
        {
            return true;
        }

        return false;
    }

    public void Tegn()
    {
        var antallXPixler = _bredde * 2 + 1;
        var antallYPixler = _høyde * 2 + 1;
        for (var y = 0; y < antallYPixler; y++)
        {
            if (y == 0)
            {
                var horisontalRammeRad = "██  " + new string('█', antallXPixler * 2 -  4);
                Console.WriteLine(horisontalRammeRad);
                continue;
            }

            if (y == antallYPixler - 1)
            {
                var horisontalRammeRad = new string('█', antallXPixler*2 - 4) + "  ██";
                Console.WriteLine(horisontalRammeRad);
                continue;
            }

            var rad = "";
            for (var x = 0; x < antallXPixler; x++)
            {
                if (x == 0 || x == antallXPixler - 1)
                {
                    // Venstre/høyre vegg
                    rad += "██";
                }
                else
                {
                    if (y % 2.0f != 0)
                    {
                        // Sti-celle-rader
                        if (x % 2 != 0)
                        {
                            // Sti-celle-kolonne
                            rad += "  ";
                        }
                        else
                        {
                            // Vegg-kolonner. 
                            if (ErTilknyttedeCeller(new Point(x/2-1,(y-1)/2), new Point(x/2,(y-1)/2)))
                            {
                                rad += "  ";
                            }
                            else
                            {
                                rad += "██";
                            }
                        }
                    }
                    else
                    {
                        // Vegg-rader
                        if (x % 2 == 0)
                        {
                            rad += "██";
                        }
                        else if (ErTilknyttedeCeller(new Point((x-1)/2, y / 2 -1), new Point((x-1)/2, y / 2)))
                        {
                            // Finnes åpning mellom sti-celle over og under
                            rad += "  ";
                        }
                        else
                        {
                            // Finnes ikke åpning mellom sti-celle over og under
                            rad += "██";
                        }
                    }
                }
            }

            Console.WriteLine(rad);
        }

        Console.ReadLine();
    }
}

class Celle
{
    public Celle(Point koordinat)
    {
        Koordinat = koordinat;
    }
    
    public Point Koordinat { get; set; }
}