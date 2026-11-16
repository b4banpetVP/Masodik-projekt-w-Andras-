
public class Fiok
{
    protected string f_nev;
    protected string f_jelszo;

    public Fiok(string nev, string jelszo)
    {
        this.f_nev = nev;
        this.f_jelszo = jelszo;
    }

    public string Nev
    {
        get => f_nev;
        set => f_nev = string.IsNullOrWhiteSpace(value)
            // if true
            ? "Nincs felhasználónév"
            // if false
            : value;
    }

    public string Jelszo
    {
        get => f_jelszo;
        set => f_jelszo = string.IsNullOrWhiteSpace(value)
            ? "Nincs jelszó"
            : value;
    }
}
