# QuizApp

En WPF-applikation i C# som l�ter anv�ndaren hantera och spela quiz. Applikationen anv�nder MongoDB f�r att 
spara quiz-data, s� att man kan skapa, l�sa, uppdatera och ta bort quiz-paket (Question Packs) samt spela quiz med tidsgr�ns.

---

## Funktionalitet

- **Quiz-paket:**  
  - Skapa nya quiz-paket, redigera dem och ta bort dem.
  - Varje paket inneh�ller en upps�ttning fr�gor med svarsalternativ, d�r varje fr�ga har ett 
	korrekt svar, flera felaktiga svar, en kategori, sv�righetsgrad och en tidsgr�ns per fr�ga.

- **Spel:**  
  - Spelaren kan spela ett quiz d�r tidtagning per fr�ga hanteras.
  - Under spelets g�ng visas fr�getid, fr�getext och svarsalternativ.  
  - Efter quizet visas en resultatdialog med antalet r�tta svar.

- **Fullscreen:**  
  - I spelvyn (PlayerView) kan du v�xla till fullscreen-l�ge vilket tar bort f�nstrets titelrad 
	och anpassar layouten f�r en b�ttre spelupplevelse.
  - Fullscreen-funktionen �r endast aktiv i spelvyn.

- **Kategori-hantering:**  
  - Varje quiz-paket kan tilldelas en kategori, vald fr�n en dropdown-meny.
  - Kategorierna lagras i MongoDB och h�mtas automatiskt n�r applikationen startar.

---

## Teknologier

- **C# / .NET (WPF):**  
  Applikationen �r byggd med Windows Presentation Foundation (WPF) i C#.

- **MongoDB:**  
  MongoDB anv�nds som databas via MongoDB.Driver f�r att lagra quiz-data.

- **Asynkrona anrop:**  
  Alla databasanrop sker asynkront med async/await f�r att s�kerst�lla att UI:t f�rblir responsivt.

- **MVVM:**  
  Applikationen f�ljer MVVM-m�nstret f�r att separera UI fr�n logik.

---

## Installation och K�rning

### F�ruts�ttningar

- .NET SDK (t.ex. .NET 6)
- MongoDB installerat och ig�ng p� localhost (standardport)
- Visual Studio (eller annan IDE med st�d f�r WPF)

### Steg

1. **Klona repot:**

2.�ppna l�sningen: �ppna projektet i Visual Studio.

3.Bygg projektet: Kompilera l�sningen. Applikationen kommer att skapa databasen 
(med ditt f�r- och efternamn) om den inte redan finns.

4.K�r applikationen: Starta applikationen med F5. Databasen skapas automatiskt och 
eventuella demodata laddas in vid f�rsta k�rningen.


### Anv�ndargr�nssnitt
Navigering:
Anv�nd menyn h�gst upp i applikationen f�r att:

- Skapa, redigera och ta bort quiz-paket.
- V�xla mellan redigeringsvyn (ConfigurationView) och spelvyn (PlayerView).
- Aktivera fullscreen-l�ge i spelvyn (Fullscreen-knappen �r endast aktiv i PlayerView).

Spel:
I spelvyn visas:

- En timer med �terst�ende tid per fr�ga.
- Fr�getext och svarsalternativ med visuell feedback (r�tt eller fel).
- Efter avslutat quiz visas en resultatdialog med spelarens antal r�tta svar.

Asynkrona Anrop
Alla databasanrop sker asynkront med hj�lp av async/await, vilket garanterar att applikationen 
f�rblir responsiv �ven under l�nga operationer mot databasen.

K�nda Begr�nsningar
- Fullscreen-l�get �r endast aktivt i spelvyn.
- MongoDB m�ste vara ig�ng p� localhost f�r att applikationen ska kunna ansluta och spara data.

- Kontakt
Om du har fr�gor eller beh�ver support, kontakta mig via e-post: tina.lagesson@gmail.com

Denna applikation �r utvecklad som en nyb�rjarimplementering av en quiz-app med MongoDB och WPF. 
Den fokuserar p� grundl�ggande CRUD-funktionalitet och en enkel men funktionell anv�ndarupplevelse.