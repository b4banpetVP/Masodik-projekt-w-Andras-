using System.IO;
using System.Security.Cryptography;
using System.Text;

// csv struktúra: Felhasználónév;Jelszó

// Titkosítás paraméterei
const int SaltMeret = 16;
const int NonceMeret = 12;
const int TagMeret = 16;
const int Iteraciok = 600_000;

void EncryptFajlba (string input, string jelszo, string eleresiUt) {
    byte[] salt     = RandomNumberGenerator.GetBytes(SaltMeret);
    byte[] nonce    = RandomNumberGenerator.GetBytes(NonceMeret);
    byte[] key      = Rfc2898DeriveBytes.Pbkdf2(jelszo, salt, Iteraciok, HashAlgorithmName.SHA256, 32);

    byte[] plain        = Encoding.UTF8.GetBytes(input);
    byte[] ciphertext   = new byte[plain.Length];
    byte[] tag          = new byte[TagMeret];

    using var aes = new AesGcm(key, TagMeret);
    aes.Encrypt(nonce, plain, ciphertext, tag);

    using var fs = File.Create(eleresiUt);
    fs.Write(salt,       0, salt.Length);
    fs.Write(nonce,      0, nonce.Length);
    fs.Write(tag,        0, tag.Length);
    fs.Write(ciphertext, 0, ciphertext.Length);
}

string DecryptFajlbol(string jelszo, string eleresiUt) {
    byte[] data = File.ReadAllBytes(eleresiUt);
    int headerSize = SaltMeret + NonceMeret + TagMeret;

    byte[] salt = data[..SaltMeret];
    byte[] nonce = data[SaltMeret..(SaltMeret + NonceMeret)];
    byte[] tag = data[(SaltMeret + NonceMeret)..headerSize];
    byte[] ciphertext = data[headerSize..];
    byte[] key = Rfc2898DeriveBytes.Pbkdf2(jelszo, salt, Iteraciok, HashAlgorithmName.SHA256, 32);
    byte[] plaintext = new byte[ciphertext.Length];

    try {
        using var aes = new AesGcm(key, TagMeret);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);
        return Encoding.UTF8.GetString(plaintext);
    } finally {
        CryptographicOperations.ZeroMemory(key);
    }
}
// kulso functions

string Kereses(string fajl_nev) {
    // felhasznalo nevek felsorolasa
    Console.WriteLine();
    Console.WriteLine("Kérem a jelszót: ");
    string keresJelszo = Console.ReadLine();
    Console.WriteLine("Felhasználó nevek:");
    int maxFelhHossz = 0;

    List<string> keresValasztasok = new List<string>();
    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split("\n")) {
        if (sor.Split(";")[0].Length > maxFelhHossz) {
            maxFelhHossz = sor.Split(";")[0].Length;
        }
        keresValasztasok.Append(sor.Split(";")[0]);
    }

    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split("\n")) {
        try {
            Console.WriteLine($"{sor.Split(";")[0]}{new string(' ', maxFelhHossz-sor.Split(";")[0].Length+1)}| {new string('*', sor.Split(";")[1].Length)}");
        } catch {}
    }
    // felhasznalo nevek felsorolasa
    // jelszo return 
    Console.WriteLine();
    Console.WriteLine("Melyik felhasználónév?");
    
    string f_nev = Console.ReadLine();
    Console.WriteLine();

    string jelszo = "";
    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split("\n")) {
        string fnev = sor.Split(";")[0];
        if (fnev == f_nev) {
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

// funtions 


if (File.Exists("fajl_nevek.txt") == false)
{
    File.WriteAllText("fajl_nevek.txt", "");
}

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



    // menupontok

    if (valasz == 1)
    {
        Console.WriteLine();
        Console.WriteLine("Mi legyen a tároló neve:");
        string fajl_nev = Console.ReadLine();

        Console.WriteLine("Kérem a jelszót: ");
        string letrehozJelszo = Console.ReadLine();
        if (fajl_nev.Contains(".bin"))
        {
            EncryptFajlba("", letrehozJelszo, fajl_nev);
        }

        else
        {
            fajl_nev += ".bin";
            EncryptFajlba("", letrehozJelszo, fajl_nev);
        }
        //global listaba elsonek filebol iratas
        File.AppendAllText("fajl_nevek.txt", $"{fajl_nev}\n");
        Console.WriteLine("Sikeres Rögzítés!");
    }

    if (valasz == 2)
    {
        Console.WriteLine();
        Console.WriteLine("A létező tárolók:");
        Console.WriteLine();

        foreach (string sor in File.ReadLines("fajl_nevek.txt"))
        {
            Console.WriteLine(sor);
            
        }

        Console.WriteLine();


        Console.WriteLine("Melyik tárolót törli?");
        string fajl_nev = Console.ReadLine();

        if (fajl_nev.Contains(".bin"))
        {
            File.Delete(fajl_nev);
        }

        else
        {
            fajl_nev += ".bin";
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

        foreach (string sor in File.ReadLines("fajl_nevek.txt"))
        {
            Console.WriteLine(sor);
            
        }

        Console.WriteLine();

        Console.WriteLine("Melyik tárolóban keres?");
        string fajl_nev = Console.ReadLine();
        // tarolo valasztas




        if (fajl_nev.Contains(".bin"))
        {
            Kereses(fajl_nev);
        }

        else
        {
            fajl_nev += ".bin";
            Kereses(fajl_nev);
        }

    }


    if (valasz == 4)
    {
        // tarolok kiiratasa
        Console.WriteLine();
        Console.WriteLine("A létező tárolók:");
        Console.WriteLine();

        foreach (string sor in File.ReadLines("fajl_nevek.txt"))
        {
            Console.WriteLine(sor);
           
        }


        Console.WriteLine();
        // tarolok kiiratasa

        // Adatok bekérése
        Console.WriteLine("Melyik tároló?");
        string fajlnev = Console.ReadLine();
        Console.WriteLine("Kérem a jelszót: ");
        string hozzaadJelszo = Console.ReadLine();
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
        string adatok = $"\n{fioknev};{fiokjelszo}";

        if (fajlnev.Contains(".bin"))
        {
            EncryptFajlba(DecryptFajlbol(hozzaadJelszo, fajlnev)+adatok, hozzaadJelszo, fajlnev);
        }

        else
        {
            fajlnev += ".bin";
            EncryptFajlba(DecryptFajlbol(hozzaadJelszo, fajlnev)+adatok, hozzaadJelszo, fajlnev);
        }
        //adat rogzites

        Console.WriteLine("Sikeres rögzítés!");
    }



    // menupontok
}
