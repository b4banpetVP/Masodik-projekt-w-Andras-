# Második projekt w/András

---

#### <ins>Andrással</ins> egy **titkosított jelszó tároló rendszert** készítünk.

Ez külön tárolási egységeket fog majd létrehozni a felhasználó által megadott nevekkel, majd felhasználóneveket és jelszavakat lehet hozzájuk adni.
A jelszavakat **titkosítva** (AES-GCM és PBKDF2 segítségével) fogja **eltárolni** a program!

Ha a felhasználó el akarja érni a jelszavait, először ki kell választania az adott tárolót a mesterjelszó megadásával, majd ezután kereshet a benne lévő felhasználónevek között.

---

## Menü

```Console
======================================

 (1) Tároló kiválasztása 

 (2) Tároló létrehozása 

 (3) A kiválasztott tároló törlése 

 (4) Tárolóban keresés 

 (5) Tárolóhoz adás 

 (6) Kilépés 

======================================
```

---

###### (1) Tároló kiválasztása
- Felsorolja a létező tárolókat, bekéri a kiválasztani kívánt tároló sorszámát, majd a hozzá tartozó mesterjelszót.
- Ha a jelszó helyes, a tároló betöltődik, és a (4) és (5) menüpontok használhatóvá válnak. *(FIGYELEM: A jelszó beírásakor a karakterek láthatóak!)*

```Console
Választása:
1

A létező tárolók:
    1. Valami.bin

Adja meg a tároló sorszámát
> 1

Adja meg a tároló jelszavát
> ********

Tároló sikeresen betöltve
```

---

###### (2) Tároló létrehozása
- Bekéri a tároló nevét (automatikusan hozzáadja a `.bin` kiterjesztést, ha nincs megadva).
- Ellenőrzi, hogy létezik-e már ilyen nevű tároló.
- Bekéri a mesterjelszót, amelynek meg kell felelnie a szigorú biztonsági feltételeknek (min. 12 karakter, kis- és nagybetű, 2 számjegy, 1 szimbólum). *(FIGYELEM: A jelszó beírásakor a karakterek láthatóak!)*

```Console
Választása:
2

Adja meg a tároló nevét
> Valami

Adja meg a tároló jelszavát
> Asd123!Asd123!

Sikeres rögzítés
```

---

###### (3) A kiválasztott tároló törlése
- Csak akkor működik, ha az (1)-es menüpontban már kiválasztottál egy tárolót.
- Megerősítést kér a felhasználótól a "TORLES" szó beírásával. Ha sikeres, a fájl és a nyilvántartásból való bejegyzés is törlődik.

```Console
Választása:
3

FIGYELEM - Ez a művelet visszaállíthatatlan adatvesztést okozhat.
A törlés megerősítéséhez írja be hogy TORLES, vagy a visszalépéshez akármi mást.
> TORLES

Sikeres törlés
```

---

###### (4) Tárolóban keresés
- Csak akkor működik, ha az (1)-es menüpontban már kiválasztottál egy tárolót.
- Felsorolja a tárolóban lévő felhasználóneveket (a jelszavakat csillagokkal elrejtve), majd bekéri a felhasználótól a kívánt fiók sorszámát.
- Kiírja a kiválasztott fiók jelszavát (cián színnel).

```Console
Választása:
4

Felhasználónevek:
    1. felhasznalo1 | ****
    2. felhasznalo2 | ********

Adja meg a felhasználónév sorszámát
> 1

A jelszava: titkosJelszo123
```

---

###### (5) Tárolóhoz adás
- Csak akkor működik, ha az (1)-es menüpontban már kiválasztottál egy tárolót.
- Bekéri az új fiók felhasználónevét, majd jelszavát, végül titkosítva hozzáadja a kiválasztott tárolóhoz. *(FIGYELEM: A jelszó beírásakor a karakterek láthatóak!)*

```Console
Választása:
5

Adja meg a fiók felhasználónevét
> ujfelhasznalo

Adja meg a fiók jelszavát
> ujJelszo456

Sikeres rögzítés
```

---

###### (6) Kilépés
- Bezárja a programot.

---

## Hibakezelés
A program a háttérben elkapja a kriptográfiai hibákat (pl. rossz mesterjelszó megadása, vagy ha a fájl megsérült), és a program összeomlása helyett a következő üzenetet írja ki:
`Hibás jelszó vagy sérült tároló`

---

# Továbbfejlesztési lehetőségek
- **Biztonságos jelszóbevitel:** A jelenlegi verzióban a jelszavak beírásakor a karakterek láthatóak a konzolon. Érdemes lenne implementálni egy függvényt, ami `*` karakterekkel helyettesíti a beírt szöveget (`Console.ReadKey(true)` használatával).
- **Ujjlenyomat-érzékelő:** Tároló mesterjelszó helyett biometrikus azonosítás használata.
- **Kód szervezése (OOP):** A kód jelenleg egyetlen `Program.cs` fájlban van. Érdemes lehet osztályokba szervezni (pl. `CryptoHelper`, `StorageManager`, `UI`).
- **Strukturáltabb adattárolás:** A `tarolok.dat` fájl helyett használhatnátok JSON-t (`System.Text.Json`) a tárolók nyilvántartására, mert az ellenállóbb a sérülésekkel szemben.