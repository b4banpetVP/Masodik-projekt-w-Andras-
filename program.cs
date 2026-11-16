using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection.Metadata.Ecma335;


// kulso functions

string Kereses(string fajl_nev)
{
    // felhasznalo nevek felsorolasa
    Console.WriteLine();
    Console.WriteLine("Felhasználó nevek:");
    foreach (string sor in File.ReadLines(fajl_nev))
    {
        string fnev = sor.Split(";")[0];
        Console.WriteLine(fnev);
    }
    // felhasznalo nevek felsorolasa

    // jelszo return 
    Console.WriteLine();
    Console.WriteLine("Melyik felhasználónév?");
    string f_nev = Console.ReadLine();
    Console.WriteLine();

    // jelszo kereses
    string jelszo = "";
    foreach (string sor in File.ReadLines(fajl_nev))
    {
        string fnev = sor.Split(";")[0];
        if (fnev == f_nev)
        {
            jelszo += sor.Split(";")[1];
        }
    }

    // jelszo kereses

    Console.Write("A jelszava:");
    Console.WriteLine(jelszo);
    return "";

    // jelszo return 
}

// kulso funcs 



while (true)
{
    // Menü
    Console.WriteLine();
    Console.WriteLine("======================================");
    Console.WriteLine();
    Console.WriteLine(" (1) Tároló létrehozása ");
    Console.WriteLine();
    Console.WriteLine(" (2) Tároló törlése ");
    Console.WriteLine();
    Console.WriteLine(" (3) Tárolóban keresés ");
    Console.WriteLine();
    Console.WriteLine(" (4) Tárolóhoz adás ");
    Console.WriteLine();
    Console.WriteLine("======================================");
    Console.WriteLine();
    Console.WriteLine("Választása:");
    int valasz = int.Parse(Console.ReadLine());

    // Menü


    // Global valtozok
    List<string> fajl_nevek = new() {};
    // Global valtozok

    // menupontok

    if (valasz == 1)
    {
        Console.WriteLine();
        Console.WriteLine("Mi legyen a tároló neve:");
        string fajl_nev = Console.ReadLine();
        if (fajl_nev.Contains(".txt"))
        {
            File.WriteAllText(fajl_nev, "");
        }

        else
        {
            fajl_nev += ".txt";
            File.WriteAllText(fajl_nev, "");
        }
        fajl_nevek.Add(fajl_nev);
        Console.WriteLine("Sikeres Rögzítés!");
    }

    if (valasz == 2)
    {
        Console.WriteLine();
        Console.WriteLine("A létező tárolók:");
        Console.WriteLine();


        //HIBA !!!!!!!!!!!!
        foreach (string nev in fajl_nevek)
        {
            Console.WriteLine(nev);
        }
        //HIBA !!!!!!!!!!!!

        Console.WriteLine();


        Console.WriteLine("Melyik tárolót törli?");
        string fajl_nev = Console.ReadLine();

        if (fajl_nev.Contains(".txt"))
        {
            File.Delete(fajl_nev);
        }

        else
        {
            fajl_nev += ".txt";
            File.Delete(fajl_nev);
        }

        Console.WriteLine();
        Console.WriteLine("Sikeres Törlés!");
    }


    if (valasz == 3)
    {
        // tarolo valasztas
        Console.WriteLine();
        Console.WriteLine("A létező tárolók");
        Console.WriteLine();

        Console.WriteLine("--------------------------------------");

        foreach (string nev in fajl_nevek)
        {
            Console.WriteLine(nev);
        }

        Console.WriteLine("--------------------------------------");
        Console.WriteLine();

        Console.WriteLine("Melyik tárolóban keres?");
        string fajl_nev = Console.ReadLine();
        // tarolo valasztas




        if (fajl_nev.Contains(".txt"))
        {
            Kereses(fajl_nev);
        }

        else
        {
            fajl_nev += ".txt";
            Kereses(fajl_nev);
        }

    }


    if (valasz == 4)
    {
        // tarolok kiiratasa
        Console.WriteLine();
        Console.WriteLine("A létező tárolók:");
        Console.WriteLine();

        

        //HIBA !!!!!!!!!!!!
        foreach (string nev in fajl_nevek)
        {
            Console.WriteLine(nev);
        }
        //HIBA !!!!!!!!!!!!

    
        Console.WriteLine();
        // tarolok kiiratasa

        // Adatok bekérése
        Console.WriteLine("Melyik tároló?");
        string fajlnev = Console.ReadLine();
        Console.WriteLine();
        Console.WriteLine("Mi a fiók neve:");
        string fioknev = Console.ReadLine();
        Console.WriteLine();
        Console.WriteLine("Mi a fiók jelszava:");
        string fiokjelszo = Console.ReadLine();
        Console.WriteLine();
        // Adatok bekérése

        //adat rogzites
        //new Fiok(fioknev,fiokjelszo); 
        string adatok = $"{fioknev};{fiokjelszo}";

        if (fajlnev.Contains(".txt"))
        {
            File.AppendAllText(fajlnev, adatok);
        }

        else
        {
            fajlnev += ".txt";
            File.AppendAllText(fajlnev, adatok);
        }
        //adat rogzites

        Console.WriteLine("Sikeres rögzítés!");
    }



    // menupontok
}



