# Arcade2D

Dwuwymiarowa gra zręcznościowa stworzona w technologii **C#**, **.NET 8** oraz **MonoGame Framework** w ramach projektu z Programowania Obiektowego.

Projekt prezentuje praktyczne zastosowanie zasad programowania obiektowego, zarządzania stanami gry, obsługi kolizji, zarządzania encjami oraz modularnej architektury oprogramowania.

---

## Opis projektu

Arcade2D to klasyczna gra zręcznościowa inspirowana mechaniką gier typu maze arcade. Gracz porusza się po planszy, zbiera przedmioty, unika przeciwników oraz zdobywa punkty.

Gra zawiera:

* menu główne,
* pełną pętlę rozgrywki,
* ekrany zwycięstwa i porażki,
* przeciwników z własną logiką zachowania,
* system bonusów,
* zapisywanie wyników,
* obsługę kolizji,
* architekturę opartą na stanach gry.

---

## Wykorzystane technologie

* **C#**
* **.NET 8**
* **MonoGame Framework 3.8**
* **OpenGL**
* **SpriteFontPlus**

---

## Architektura

Projekt został zaprojektowany zgodnie z zasadami programowania obiektowego oraz podziału odpowiedzialności pomiędzy poszczególne moduły.

### Główne komponenty

#### Encje (Entities)

Hierarchia obiektów występujących w grze:

* `Entity`
* `Player`
* `Ghost`
* `Pellet`
* `PowerPellet`
* `SpeedPellet`
* `Wall`

#### Menedżery (Managers)

Komponenty odpowiedzialne za logikę gry:

* `EntityManager`
* `CollisionManager`
* `ScoreManager`

#### Stany gry (States)

System zarządzania przebiegiem gry:

* `MenuState`
* `GameplayState`
* `VictoryState`
* `GameOverState`

#### Narzędzia pomocnicze (Utils)

Klasy wspierające działanie gry oraz funkcje pomocnicze wykorzystywane w różnych modułach projektu.

---

## Funkcjonalności

### Rozgrywka

* płynne sterowanie postacią,
* zbieranie przedmiotów,
* unikanie przeciwników,
* system punktacji,
* warunki zwycięstwa i przegranej.

### System kolizji

Dedykowany moduł odpowiada za obsługę interakcji pomiędzy obiektami gry:

* kolizje ze ścianami,
* zbieranie przedmiotów,
* kontakt z przeciwnikami.

### Bonusy

W grze zaimplementowano specjalne przedmioty zapewniające tymczasowe efekty:

* **Power Pellet**
* **Speed Pellet**

Bonusy wpływają na możliwości gracza i wzbogacają rozgrywkę o dodatkowe elementy strategiczne.

### Zapisywanie wyników

Wyniki gracza są automatycznie zapisywane i odczytywane pomiędzy kolejnymi uruchomieniami gry.

---

## Struktura projektu

```text
Arcade2D
├── Content/
├── Entities/
├── Managers/
├── States/
├── Utils/
├── Graphics/
├── docs/
├── Game1.cs
├── Program.cs
└── Arcade2D.csproj
```

---

## Uruchomienie projektu

### Wymagania

Przed uruchomieniem projektu należy zainstalować:

* .NET 8 SDK
* MonoGame Framework
* Sterowniki graficzne zgodne z OpenGL

---

## Dokumentacja

Dodatkowa dokumentacja znajduje się w katalogu `docs/`.

Obejmuje ona między innymi:

* diagramy UML,
* diagramy komponentów,
* diagramy sekwencji,
* dokumentację architektury systemu,
* notatki projektowe i plany rozwoju.

---

## Założenia projektowe

Projekt został opracowany z uwzględnieniem następujących zasad:

* Programowanie obiektowe (OOP),
* zasady SOLID,
* separacja odpowiedzialności (Separation of Concerns),
* modularna architektura,
* łatwość rozbudowy i utrzymania kodu,
* ponowne wykorzystanie komponentów.

Architektura umożliwia łatwe dodawanie nowych przeciwników, bonusów oraz mechanik rozgrywki bez konieczności znaczących zmian w istniejącym kodzie.

---

## Możliwe kierunki rozwoju

Planowane lub potencjalne rozszerzenia projektu:

* wiele poziomów gry,
* bardziej zaawansowana sztuczna inteligencja przeciwników,
* efekty dźwiękowe i muzyka,
* tabela najlepszych wyników,
* dodatkowe bonusy i przedmioty,
* system zapisu stanu gry.

---

## Autor

**Anastasiia Tsyban**