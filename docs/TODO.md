# TODO / Roadmap

## Aktualny stan projektu

Projekt jest inspirowany klasycznym Pac-Manem i został stworzony w technologii MonoGame z wykorzystaniem programowania obiektowego (OOP).

### Zaimplementowano

* architekturę opartą o OOP,
* system encji (Entity System),
* system renderowania obiektów,
* ruch gracza,
* system kolizji,
* system ścian (Wall System),
* centralną paletę kolorów,
* ładowanie mapy z pliku,
* zarządzanie stanami gry (Menu, Gameplay, Victory, Game Over),
* zarządzanie encjami przez EntityManager,
* zarządzanie kolizjami przez CollisionManager,
* wykorzystanie LINQ do filtrowania i wyszukiwania encji.

---

# Gameplay

## Zaimplementowano

* [x] pełny system labiryntu (Maze System),
* [x] system pelletów,
* [x] system punktacji,
* [x] podstawowe AI duchów,
* [x] Power Pellets,
* [x] warunki zwycięstwa i przegranej,
* [x] restart gry,
* [x] licznik punktów wyświetlany w interfejsie.

## Do wykonania

### Rozszerzenie mechanik gry

* [ ] dodanie kilku typów zachowań duchów:

  * duch ścigający gracza,
  * duch poruszający się losowo,
  * duch próbujący przewidzieć ruch gracza.

* [ ] system poziomów trudności:

  * zwiększanie prędkości duchów wraz z czasem,
  * zwiększanie liczby aktywnych przeciwników.

* [ ] system żyć gracza:

  * 3 życia na początku rozgrywki,
  * utrata życia po kontakcie z duchem,
  * Game Over po utracie wszystkich żyć.

### Power-Upy

* [ ] licznik czasu działania każdego Power Pelleta,
* [ ] wizualna informacja o aktywnym bonusie,
* [ ] możliwość łatwego dodawania nowych bonusów.

Planowane bonusy:

* [ ] Freeze Pellet – zatrzymanie duchów,
* [ ] Speed Pellet – zwiększenie prędkości gracza,
* [ ] Double Score Pellet – podwójne punkty przez określony czas.

---

# Architektura

## Zaimplementowano

* [x] EntityManager,
* [x] CollisionManager,
* [x] system ładowania map,
* [x] zarządzanie stanami gry,
* [x] wykorzystanie LINQ.

## Do wykonania

### Refaktoryzacja

* [x] wydzielenie zarządzania wynikami do osobnego ScoreManagera,
* [x] ograniczenie odpowiedzialności klasy Game1.

### Programowanie asynchroniczne

* [x] asynchroniczny zapis wyników do pliku,
* [ ] asynchroniczne wczytywanie danych gry,
* [ ] wykorzystanie async/await w miejscach operujących na plikach.

### Rozszerzenie architektury

* [ ] Factory Pattern dla tworzenia encji,
* [ ] Event System dla komunikacji między systemami,
* [ ] uproszczenie dodawania nowych typów pelletów.

---

# Grafika i UI

## Zaimplementowano

* [x] menu główne,
* [x] ekran zwycięstwa,
* [x] ekran przegranej,
* [x] licznik punktów.

## Do wykonania

### Grafika

* [x] zastąpienie placeholderów docelowymi sprite'ami,
* [x] sprite gracza,
* [x] sprite'y duchów,
* [x] animacje ruchu.

### Interfejs użytkownika

* [ ] wyświetlanie liczby żyć,
* [x] poprawa układu HUD.

### Audio (dodam po wszystkich zaplanowanych features)

* [ ] efekty dźwiękowe:

  * zbieranie pelletów,
  * aktywacja bonusów,
  * śmierć gracza,
  * zwycięstwo.

* [ ] muzyka w tle,

* [ ] regulacja głośności.

---

# Dokumentacja

## Do wykonania

### UML

* [ ] diagram klas UML,
* [ ] diagram dziedziczenia encji,
* [ ] diagram zależności między managerami.

### Dokumentacja kodu

* [ ] opis wszystkich klas,
* [ ] opis wszystkich interfejsów,
* [ ] opis wzorców projektowych,
* [ ] opis odpowiedzialności poszczególnych managerów.

### Dokumentacja techniczna

* [ ] opis architektury projektu,
* [ ] opis systemu kolizji,
* [ ] opis systemu encji,
* [ ] opis zarządzania stanami gry,
* [ ] opis procesu renderowania,
* [ ] opis Game Loop.

### Learning Notes

* [ ] czego nauczyłam się podczas pracy z MonoGame,
* [ ] wykorzystanie OOP w praktyce,
* [ ] zastosowanie interfejsów,
* [ ] wykorzystanie LINQ,
* [ ] implementacja wzorców projektowych,
* [ ] napotkane problemy i sposoby ich rozwiązania.

---

# Priorytety przed oddaniem projektu

## Wysoki priorytet

* [х] sprite'y i animacje,
* [ ] timer gry,
* [х] timery bonusów,
* [ ] diagram UML,
* [ ] opis architektury,
* [ ] learning notes.

## Średni priorytet

* [ ] system żyć,
* [ ] efekty dźwiękowe,
* [ ] ulepszone AI duchów.

## Niski priorytet

* [ ] muzyka w tle,
* [ ] Factory Pattern,
* [ ] Event System.