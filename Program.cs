using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
    Console.WriteLine("Felhasználónevek:");

    List<(string Felhasznalonev, string Jelszo)> fiokok = [];
    foreach (string sor in DecryptFajlbol(keresJelszo, fajl_nev).Split('\n')) {
        string[] mezok = sor.TrimEnd('\r').Split(CsvSeparator);
        if (mezok.Length >= 2 && mezok[0].Length > 0) {
            fiokok.Add((mezok[0], mezok[1]));
        }
    }

    if (fiokok.Count == 0) {
        Console.WriteLine("A tárolóban nincs megjeleníthető fiók.");
        return "";
    }

    int maxFelhHossz = fiokok.Max(fiok => fiok.Felhasznalonev.Length);
    for (int i = 0; i < fiokok.Count; i++) {
        var fiok = fiokok[i];
        Console.WriteLine($"\t{i + 1}. {fiok.Felhasznalonev}{new string(' ', maxFelhHossz - fiok.Felhasznalonev.Length + 1)}| {new string('*', fiok.Jelszo.Length)}");
    }

    Console.WriteLine();
    Console.WriteLine("Adja meg a felhasználónév sorszámát");
    Console.Write("> ");
    string valasztas = Console.ReadLine() ?? "";
    if (!int.TryParse(valasztas, out int valasztottSorszam) ||
        valasztottSorszam < 1 ||
        valasztottSorszam > fiokok.Count) {
        Console.Clear();
        Console.WriteLine("\nÉrvénytelen sorszám");
        return("");
    }

    Console.Clear();
    Console.Write("A jelszava: ");

    ConsoleColor eredetiSzin = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(fiokok[valasztottSorszam - 1].Jelszo);
    Console.ForegroundColor = eredetiSzin;

    Console.WriteLine("\nNyomja meg az entert a jelszó elrejtéséhez és továbblépéshez");
    Console.Read();
    return "";
}

while (true) {
    fajl_nevek = [.. File.ReadAllText("tarolok.dat").Split(CsvSeparator)];
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
        Console.WriteLine("Választása");
        Console.Write("> ");
        string input = Console.ReadLine() ?? "";

        // Sorszámozó
        Dictionary<string, int> tarolok = [];
        Dictionary<int, string> tarolokinv = [];
        int sorszam = 0;
        foreach (string sor in File.ReadAllText("tarolok.dat").Split(CsvSeparator)) {
            if (sor != "") {
                sorszam += 1;
                tarolok.Add(sor, sorszam);
                tarolokinv.Add(sorszam, sor);
            }
        }

        // Input validálás
        if (int.TryParse(input, out int valasz)) {
            // Menüpontok (1 - 6)
            switch (valasz) {
                // 1. Tároló kiválasztása
                case 1:
                    Console.Clear();
                    Console.WriteLine("A létező tárolók:");

                    // Sorszám értelmezése és tárolók listázása
                    foreach (string sor in File.ReadAllText("tarolok.dat").Split(CsvSeparator)) {
                        if (sor != "") {
                            tarolok.TryGetValue(sor, out int taroloszam);
                            Console.WriteLine($"\t{taroloszam}. {sor}");
                        }
                    }

                    // Adatok bekérése
                    Console.WriteLine("\nAdja meg a tároló sorszámát");
                    Console.Write("> ");
                    valaszottTaroloSorszamStrTemp = Console.ReadLine() ?? "";
                    if (!int.TryParse(valaszottTaroloSorszamStrTemp, out int value) ||
                        value < 1 ||
                        value > tarolok.Count) {
                        Console.Clear();
                        Console.WriteLine("\nHelytelen formátumú bemenet");
                    } else {
                        valaszottTaroloSorszam = value;
                        Console.WriteLine("\nAdja meg a tároló jelszavát\n[FIGYELEM - Ebbe a mezőbe írt karakterek NEM lesznek elrejtve.]");
                        Console.Write("> ");
                        valaszottTaroloJelszo = Console.ReadLine() ?? "";
                        try {
                            string jelszoValidalasFajlnev = tarolokinv[valaszottTaroloSorszam];
                            DecryptFajlbol(valaszottTaroloJelszo, jelszoValidalasFajlnev);
                            Console.Clear();
                            Console.WriteLine("\nTároló sikeresen betöltve");
                        } catch (CryptographicException) {
                            throw;
                        }
                    }
                break;

                // 2. Tároló létrehozása
                case 2:
                    Console.Clear();
                    Console.WriteLine("Adja meg a tároló nevét\n(tartalmazhatja a .bin fájlkiterjesztést, de ez nem kötelező)");
                    Console.Write("> ");
                    string fajl_nev = Console.ReadLine() ?? "";

                    if (!fajl_nev.Contains(".bin")) {
                        fajl_nev += ".bin";
                    }

                    if (fajl_nevek.Contains(fajl_nev)) {
                        Console.Clear();
                        Console.WriteLine("\nEgy tároló már létezik ezzel a névvel");
                        break;
                    }

                    Console.WriteLine(
                        "\nAdja meg a tároló jelszavát\n" +
                        "A jelszónak legalább 12 karakteresnek kell lennie, és tartalmaznia kell nagybetűt, kisbetűt, legalább két számjegyet és egy szimbólumot.\n" +
                        "[FIGYELEM - Ebbe a mezőbe írt karakterek NEM lesznek elrejtve.]");
                    Console.Write("> ");
                    string letrehozJelszo = Console.ReadLine() ?? "";

                    // A Unicode kategóriák miatt bármely nyelv betűi és számjegyei használhatók.
                    // A mintának legalább 12 karaktert, nagybetűt, kisbetűt, két számjegyet és egy szimbólumot kell tartalmaznia.
                    if (!Regex.IsMatch(
                            letrehozJelszo,
                            @"\A
                              (?=.{12,}\z)
                              (?=.*\p{Lu})
                              (?=.*\p{Ll})
                              (?=(?:.*\p{Nd}){2,})
                              (?=.*[\p{P}\p{S}])
                              .*\z",
                            RegexOptions.IgnorePatternWhitespace)) {
                        Console.Clear();
                        Console.WriteLine("\nA jelszónak legalább 12 karakteresnek kell lennie, és tartalmaznia kell nagybetűt, kisbetűt, legalább két számjegyet és egy szimbólumot.");
                        break;
                    }

                    EncryptFajlba("", letrehozJelszo, fajl_nev);
                    File.AppendAllText("tarolok.dat", $"{fajl_nev}{CsvSeparator}");
                    Console.Clear();
                    Console.WriteLine("\nSikeres rögzítés");
                break;

                // 3. A kiválasztott tároló törlése
                case 3:
                    Console.Clear();
                    Console.WriteLine("FIGYELEM - Ez a művelet visszaállíthatatlan adatvesztést okozhat.\nA törlés megerősítéséhez írja be hogy TORLES, vagy a visszalépéshez akármi mást.");
                    Console.Write("> ");
                    if (Console.ReadLine() == "TORLES") {
                        if (tarolokinv.TryGetValue(valaszottTaroloSorszam, out string? torlesFajlnev)) {
                            File.Delete(torlesFajlnev);
                            fajl_nevek.Remove(torlesFajlnev);
                            File.WriteAllText("tarolok.dat", String.Join(CsvSeparator, fajl_nevek));
                            valaszottTaroloSorszam = 0;
                            valaszottTaroloJelszo = "";
                            Console.Clear();
                            Console.WriteLine("\nSikeres törlés");
                        } else {
                            Console.WriteLine("\nElőbb válasszon ki egy tárolót");
                        }
                    }
                break;

                // 4. Tárolóban keresés
                case 4:
                    if (tarolokinv.TryGetValue(valaszottTaroloSorszam, out string? keresesFajlnev)) {
                        Kereses(keresesFajlnev);
                    } else {
                        Console.Clear();
                        Console.WriteLine("\nElőbb válasszon ki egy tárolót");
                    }
                break;

                // 5. Tárolóhoz adás
                case 5:
                    if (tarolokinv.TryGetValue(valaszottTaroloSorszam, out string? fajlnev)) {
                        Console.Clear();
                        Console.WriteLine("Adja meg a fiók felhasználónevét");
                        Console.Write("> ");
                        string fioknev = Console.ReadLine() ?? "";
                        Console.WriteLine("Adja meg a fiók jelszavát\n[FIGYELEM - Ebbe a mezőbe írt karakterek NEM lesznek elrejtve.]");
                        Console.Write("> ");
                        string fiokjelszo = Console.ReadLine() ?? "";
                        string adatok = $"{fioknev}{CsvSeparator}{fiokjelszo}\n";
                        EncryptFajlba(DecryptFajlbol(valaszottTaroloJelszo, fajlnev) + adatok, valaszottTaroloJelszo, fajlnev);
                        Console.Clear();
                        Console.WriteLine("\nSikeres rögzítés");
                    } else {
                        Console.WriteLine("\nElőbb válasszon ki egy tárolót");
                    }
                break;

                // 6. Kilépés
                case 6:
                    Console.Clear();
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
