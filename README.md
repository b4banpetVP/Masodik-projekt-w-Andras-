
# Második projekt w/András

---

#### <ins>Andrással</ins> egy **titkosított jelszó tároló rendszert** készítünk.

Ez külön tárolási egységeket fog majd létrehozni a felhasználó által megadott nevekkel, majd felhasználóneveket és jelszavakat lehet hozzájuk adni.
A jelszavakat **titkosítva** fogja **eltárol**ni a program!

Ha a felhasználó el akarja érni a jelszavait, kiválasztja az adott tárolót, majd választ a felhasználó nevek közül.

---

## Menü

```Console
======================================

 (1) Tároló létrehozása

 (2) Tároló törlése

 (3) Tárolóban keresés

 (4) Tárolóhoz adás

======================================
```

---

###### (1) Tároló létrehozása

- Bekéri a tároló nevét, majd létrehozza azt.

```Console
Választása:
1

Mi legyen a tároló neve:
Valami

Sikeres Rögzítés!
```

---

###### (2) Tároló törlése

- Felsorolja a tárolók neveit, majd bekéri a felhasználótól melyiket szeretné törölni.

```Console
Választása:
2

A létező tárolók:
    a tárolok nevei

Melyik tárolót törli?
input

Sikeres Törlés!
```

---

###### (3) Tárolóban keresés

- Felsorolja a tárolók neveit, majd bekéri a felhasználótól melyikben szeretne keresni.
- Ezután a tárolóban lévő neveket sorolja fel és bekéri a felhasználótól melyik jelszavát kéri.

```Console
Választása:
3

A létező tárolók:
    a tárolok nevei
  
Melyik tárolóban keres?

Felhasználó nevek:
    felsorolja tárolóban levő neveket

Melyik felhasználónév?

A jelszava: a felhasznalo jelszava
```

---

###### (4) Tárolóhoz adás

- Felsorolja a tárolók neveit, majd bekéri a felhasználótól melyikhez szeretne hozzáadni.
- Ezután bekéri a felhasználó fiók nevét, majd jelszavát.

```Console
Választása:
4

A létező tárolók:
    a tárolok nevei

Melyik tároló?

Mi a fiók neve:

Mi a fiók jelszava:

Sikeres rögzítés!
```

---

# Továbbfejlesztési lehetőségek
 - Tároló mesterjelszó helyett ujjlenyomatérzékelő használata