using System.IO;
using System.Security.Cryptography;
using System.Text;

// Tároló lista fájl létezésének biztosítása
if (File.Exists("tarolok.dat") == false) {
    File.WriteAllText("tarolok.dat", "");
}
List<string> fajl_nevek;

// CSV Struktúra: Platform | Felhasználónév | Jelszó | Hozzáadás dátuma
// Jegyzet: Valójában nem pontos vesszőt használ separatornak, hanem a(z) \x1F egység elválasztó karaktert

// Titkosítás paraméterei
const int SaltMeret = 16;
const int NonceMeret = 12;
const int TagMeret = 16;
const int Iteraciok = 600_000;

const string CsvSeparator = "\x1F";

void EncryptFajlba(string input, string jelszo, string eleresiUt) {
    byte[] salt = RandomNumberGenerator.GetBytes(SaltMeret);
    byte[] nonce = RandomNumberGenerator.GetBytes(NonceMeret);
    byte[] key = Rfc2898DeriveBytes.Pbkdf2(jelszo, salt, Iteraciok, HashAlgorithmName.SHA256, 32);

    byte[] plain = Encoding.UTF8.GetBytes(input);
    byte[] ciphertext = new byte[plain.Length];
    byte[] tag = new byte[TagMeret];

    using var aes = new AesGcm(key, TagMeret);
    aes.Encrypt(nonce, plain, ciphertext, tag);

    using var fs = File.Create(eleresiUt);
    fs.Write(salt, 0, salt.Length);
    fs.Write(nonce, 0, nonce.Length);
    fs.Write(tag, 0, tag.Length);
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
string valaszottTaroloSorszamStrTemp = "";
int valaszottTaroloSorszam = 0;
string valaszottTaroloJelszo = "";

string Kereses(string fajl_nev) {
    string keresJelszo = valaszottTaroloJelszo;
    Console.WriteLine();
    Console.WriteLine("Felhasználó nevek:");
    Console.WriteLine();
    int maxFelhHossz = 0;

    List<string> keresValasztasok = [];
    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split("\n")) {
        if (sor.Split(CsvSeparator)[0].Length > maxFelhHossz) {
            maxFelhHossz = sor.Split(CsvSeparator)[0].Length;
        }
        keresValasztasok.Add(sor.Split(CsvSeparator)[0]);
    }

    Dictionary<string, int> fiokok = [];
    Dictionary<int, string> fiokokinv = [];
    int sorszam = 0;
    foreach (string sor in keresValasztasok) {
        sorszam += 1;
        fiokok.Add(sor, sorszam);
        fiokokinv.Add(sorszam, sor);
    }

    sorszam = 0;
    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split("\n")) {
        try {
            sorszam += 1;
            Console.WriteLine($"{sorszam}. {sor.Split(CsvSeparator)[0]}{new string(' ', maxFelhHossz - sor.Split(CsvSeparator)[0].Length + 1)}| {new string('*', sor.Split(CsvSeparator)[1].Length)}");
        } catch {}
    }
    // felhasznalo nevek felsorolasa
    // jelszo return 
    Console.WriteLine();
    Console.WriteLine("Adja meg a felhasználónév sorszámát");
    Console.Write("> ");
    fiokokinv.TryGetValue(int.Parse(Console.ReadLine()), out string valasztottFelhnev);

    string jelszo = "";
    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split("\n")) {
        string fnev = sor.Split(CsvSeparator)[0];
        if (fnev == valasztottFelhnev) {
            jelszo += sor.Split(CsvSeparator)[1];
        }
    }

    // jelszo kereses
    Console.Clear();
    Console.Write("A jelszava: ");
    Console.WriteLine(jelszo);
    return "";

    // jelszo return 
}

while (true) {
    fajl_nevek = new List<string>(File.ReadAllText("tarolok.dat").Split(CsvSeparator));
    try {
        // Menü
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine();
        Console.WriteLine(" (1) Tároló kiválasztása ");
        Console.WriteLine();
        Console.WriteLine(" (2) Tároló létrehozása ");
        Console.WriteLine();
        Console.WriteLine(" (3) A kiválasztott tároló törlése ");
        Console.WriteLine();
        Console.WriteLine(" (4) Tárolóban keresés ");
        Console.WriteLine();
        Console.WriteLine(" (5) Tárolóhoz adás ");
        Console.WriteLine();
        Console.WriteLine(" (6) Kilépés ");
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine();
        Console.WriteLine("Választása:");
        string input = Console.ReadLine();

        // Sorszámozó
        Dictionary<string, int> tarolok = [];
        Dictionary<int, string> tarolokinv = [];
        int sorszam = 0;
        foreach (string sor in File.ReadAllText("tarolok.dat").Split(CsvSeparator)) {
            sorszam += 1;
            tarolok.Add(sor, sorszam);
            tarolokinv.Add(sorszam, sor);
        }

        // Input validálás
        if (int.TryParse(input, out int valasz)) {
            // Menüpontok (1 - 6)
            switch (valasz) {
                // 1. Tároló kiválasztása
                case 1:
                    Console.WriteLine("\nA létező tárolók:\n");

                    // Sorszám értelmezése és tárolók listázása
                    foreach (string sor in File.ReadAllText("tarolok.dat").Split(CsvSeparator)) {   
                        tarolok.TryGetValue(sor, out int taroloszam);
                        Console.WriteLine($"{taroloszam}. {sor}");
                    }
                    Console.WriteLine();

                    // Adatok bekérése
                    Console.WriteLine("Adja meg a tároló sorszámát");
                    Console.Write("> ");
                    valaszottTaroloSorszamStrTemp = Console.ReadLine() ?? "";
                    if (!int.TryParse(valaszottTaroloSorszamStrTemp, out int value)) {
                        Console.Clear();
                        Console.WriteLine("\nHelytelen formátumú bemenet");
                    } else {
                        valaszottTaroloSorszam = value;
                        Console.WriteLine("Adja meg a tároló jelszavát\n[FIGYELEM - Ebbe a mezőbe írt karakterek NEM lesznek elrejtve.]");
                        Console.Write("> ");
                        valaszottTaroloJelszo = Console.ReadLine() ?? "";
                        try {
                            tarolokinv.TryGetValue(valaszottTaroloSorszam, out string jelszoValidalasFajlnev);
                            DecryptFajlbol(valaszottTaroloJelszo, jelszoValidalasFajlnev);
                        } catch (CryptographicException) {
                            throw;
                        }
                    }
                break;

                // 2. Tároló létrehozása
                case 2:
                    Console.WriteLine("Adja meg a tároló nevét\n(tartalmazhatja a .bin fájlkiterjesztést, de ez nem kötelező)");
                    Console.Write("> ");
                    string fajl_nev = Console.ReadLine() ?? "";

                    if (!fajl_nev.Contains(".bin")) {
                        fajl_nev += ".bin";
                    }

                    if (fajl_nevek.Contains($"{fajl_nev}\n")) {
                        Console.Clear();
                        Console.WriteLine("Egy tároló már létezik ezzel a névvel");
                    }

                    Console.WriteLine("Adja meg a tároló jelszavát\n[FIGYELEM - Ebbe a mezőbe írt karakterek NEM lesznek elrejtve.]");
                    Console.Write("> ");
                    string letrehozJelszo = Console.ReadLine() ?? "";
                    EncryptFajlba("", letrehozJelszo, fajl_nev);
                    File.AppendAllText("tarolok.dat", $"{fajl_nev}{CsvSeparator}");
                    Console.Clear();
                    Console.WriteLine("Sikeres rögzítés");
                break;

                // 3. A kiválasztott tároló törlése
                case 3:
                    Console.WriteLine("FIGYELEM - Ez a művelet visszaállíthatatlan adatvesztést okozhat.\nA törlés megerősítéséhez írja be hogy TORLES, vagy a visszalépéshez akármi mást.");
                    Console.Write("> ");
                    if (Console.ReadLine() == "TORLES") {
                        tarolokinv.TryGetValue(valaszottTaroloSorszam, out string torlesFajlnev);
                        File.Delete(torlesFajlnev);
                        fajl_nevek.Remove(torlesFajlnev);
                        File.WriteAllText("tarolok.dat", Join(CsvSeparator));
                        Console.Clear();
                        Console.WriteLine("Sikeres törlés");
                    }
                break;

                // 4. Tárolóban keresés
                case 4:
                    tarolokinv.TryGetValue(valaszottTaroloSorszam, out string keresesFajlnev);
                    Kereses(keresesFajlnev);
                break;

                // 5. Tárolóhoz adás
                case 5:
                    tarolokinv.TryGetValue(valaszottTaroloSorszam, out string fajlnev);
                    Console.WriteLine("Adja meg a fiók felhasználónevét");
                    Console.Write("> ");
                    string fioknev = Console.ReadLine() ?? "";
                    Console.WriteLine("Adja meg a fiók jelszavát\n[FIGYELEM - Ebbe a mezőbe írt karakterek NEM lesznek elrejtve.]");
                    Console.Write("> ");
                    string fiokjelszo = Console.ReadLine() ?? "";
                    string adatok = $"{fioknev}{CsvSeparator}{fiokjelszo}\n";
                    EncryptFajlba(DecryptFajlbol(valaszottTaroloJelszo, fajlnev) + adatok, valaszottTaroloJelszo, fajlnev);
                    Console.Clear();
                    Console.WriteLine("Sikeres rögzítés");
                break;

                // 6. Kilépés
                case 6:
                    return;
            }
        } else {
            Console.Clear();
            Console.WriteLine("\nHelytelen formátumú bemenet");
        }
    // Kriptográfiával kapcsolatos hiba elkapása
    } catch (CryptographicException) {
        Console.Clear();
        Console.WriteLine("\nHibás jelszó vagy sérült tároló");
    }
}
