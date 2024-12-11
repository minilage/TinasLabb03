Labb 3 - Quiz Configurator
Vi bygger en applikation f�r att konfigurera quiz-fr�gor, och k�ra quiz-rundor. Appen ska skrivas i WPF och XAML, och byggas p� Model-View-ViewModel (MVVM)-arkitektur.


MVVM �r ett arkitektoniskt designm�nster i mjukvaruutveckling, som underl�ttar separationen av utvecklingen av ett grafiskt anv�ndargr�nssnitt fr�n den underliggande back-end logiken. Vi kommer gemensamt g� igenom hur detta fungerar.
Eftersom vi har ganska begr�nsad tid, cirka 3 veckor, f�r att bygga projektet, och vi �nnu inte g�tt igenom alla delar som kommer beh�vas, s� kommer labben vara upplagd p� s� vis att vi, delvis, g�r den gemensamt som en code-along, s� att alla f�r grundstrukturen r�tt.
Vi kommer �ven ha gemensam handledning / Q&A d�r vi gemensamt kollar p� att l�sa de problem som uppst�r f�r er under utvecklingens g�ng.
Jag har byggt ihop en (mer eller mindre) f�rdig version av projektet som ni kommer f� tillg�ng till redan fr�n b�rjan, s� att ni sj�lva kan prova att anv�nda den f�rdiga mjukvaran och fundera kring hur jag l�st olika delar i designen. Det �r inget krav p� att era l�sningar ska se ut exakt som mina, utan se det mer som inspiration. S� l�nge er app uppfyller kraven f�r G, respektive VG, s� kan ni v�lja att utforma den p� ett annat s�tt.
L�t oss b�rja med att kolla p� appen jag byggt!
Vi kommer bygga ihop appen fr�n b�rjan, gemensamt, som en code-along med start fr�n n�sta kurstillf�lle. Vi kommer anv�nda oss av MVVM och Data binding, s� koden kommer att vara annorlunda strukturerad �n i uppgifterna vi gjort hittills. D�rf�r f�resl�r jag att ni v�ntar med att b�rja skriva C#-kod tills vi s�tter ig�ng och g�r det tillsammans.
Det ni d�remot kan b�rja med �r att strukturera upp och skriva XAML-kod f�r de olika komponenterna ni kan se i min app.
Skapa en mapp i ert VisualStudio-projekt som ni d�per till �Dialogs�; i denna kan ni skapa 2 nya f�nster (Add->Window (WPF)): PackOptionsDialog.xaml, och CreateNewPackDialog.xaml; Om ni kollar i min app s� �r det dialogrutorna man f�r upp n�r man v�ljer Edit->Pack Options, respektive File->New Question Pack i menyn.
Skapa en annan mapp i ert VS-projekt som ni d�per till �Views�; i denna kan ni skapa 3 nya UserControls: MenuView.xaml (i denna bygger ni programmets huvudmeny), ConfigurationView.xaml (denna inneh�ller de komponenter man ser n�r man �r i konfigurationsl�get (View->Edit)); PlayerView.xaml (denna inneh�ller de komponenter man ser n�r man �r i spell�get (View->Play)).
MainWindow.xaml kommer i princip best� av ovanst�ende kontroller p� s�vis att MenuView alltid kommer visas h�gst upp, och ConfigurationView ELLER PlayerView visas i resten av f�nstret, beroende p� vilket l�ge man �r i. (F�r tillf�llet kan bara kommentera ut den ena eller andra f�r att testa hur det kommer se ut i respektive l�ge).
OBS! Det finns inget krav p� att dessa m�ste vara klara tills vi b�rjar bygga gemensamt med code-along, utan detta �r bara f�rslag om ni vill b�rja n�gonstans; s� ni har lite f�rberett n�r vi kommer till de delarna. OCH, som sagt, implementera inte funktionalitet i C# / Code-behinds �n, utan bygg bara upp struktur p� komponenterna i XAML.

�versikt �ver appens uppbyggnad
Appen best�r i huvudsak av tv� delar (f�r G-niv�n): en �configurator� f�r att skapa paket med fr�gor; och en �player� d�r man kan k�ra quiz-rundor. F�r VG-niv�n finns �ven en tredje del som ni hittar i menyn under File->Import Questions; denna del kopplar upp sig mot ett API online f�r att importera fr�gor fr�n deras databas. Observera att denna delen inte �r obligatorisk. F�r godk�nt beh�ver all funktionalitet F�RUTOM import via API fungera.

Configuration Mode
Man ska kunna bygga �Question Packs�, det vill s�ga �paket� med fr�gor. Man beh�ver kunna l�gga till, ta bort, och redigera befintliga fr�gor. Alla fr�gor har fyra alternativ, varav ett korrekt. Man ska �ven kunna �ndra inst�llningar f�r sj�lva paketet �Pack Options�: V�lja namn, m�rka upp med sv�righetsgrad, samt s�tta tidsgr�ns p� fr�gorna. Man ska �ven kunna skapa nya �Packs�, och ta bort befintliga.
Det ska g� att l�gga till / ta bort fr�gor p� flera s�tt: Via menyn, Via snabbknappar p� tangentbordet, och via knappar i appen. �ven �Pack Options� ska g� att �ppna p� alla 3 s�tt.

Play Mode
N�r man startar play mode ska den visa hur m�nga fr�gor det �r i det aktiva �Fr�gepaketet�, samt vilken fr�ga man �r p� i turordning. Man ska allts� svara p� alla fr�gor i paketet innan man f�r ett resultat som visar hur m�nga r�tt man hade. Ordningen fr�gorna visas i ska dock slumpas fr�n g�ng till g�ng; likadant med ordningen som svarsalternativen visas i. Det ska finnas n�gon form av timer som r�knar ner (bet�nketid per fr�ga ska g� att st�lla in n�r man konfigurerar packen), och efter man klickat p� ett svar (eller tiden tar slut) ska anv�ndaren f� feedback p� om man svarat r�tt/fel, samt vilket det korrekta svarsalternativet �r.
Det beh�ver dock inte se ut som i mitt exempel s� l�nge funktionaliteten ovan finns p� plats. Vill man till exempel visa tiden i form av en cirkel/klockvisare, eller markera svarsalternativen med olika f�rger p� knapparna ist�llet f�r ikoner s� g�r det bra.

Menyn
Menyn ska ha ikoner f�r de olika alternativen (t.ex med font-awesome). Meny alternativen ska g� att aktivera fr�n tangentbordet b�de med snabbvalsknappar, och med alt-knappen, till exempel Ctrl+O, samt Alt+E, O f�r �Pack Options�.


Json
Appen ska spara paket och fr�gor i Json, s� att man kan ladda in dem vid ett senare tillf�lle. 
Du kan v�lja att g�ra detta p� ett av f�ljande tv� s�tt:

1.	Skapa en json fil i en mapp under anv�ndarens AppData\Local som lagrar samtliga �question packs� och laddar in dem automatiskt n�r programmet startar. Detta �r alternativet jag gjort i min app. F�r att hitta mappen kan du anv�nda:
Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
T�nk p� att programmet ska fungera �ven om mappen och/eller filen inte finns; den m�ste i s� fall skapa upp mapp / fil.

2.	Anv�nd OpenFileDialog() / SaveFileDialog() och l�t anv�ndaren spara/ladda varje �question pack� i en separat fil.
Oavsett alternativ s� ska filerna sparas och l�sas in asynkront.

Full Screen
Det ska finnas ett menyalternativ f�r att k�ra programmet i helsk�rmsl�ge.


Importera fr�gor via API ( VG-uppgift )
Open Trivia Database tillhandah�ller quiz-fr�gor via ett �ppet API p� https://opentdb.com

Anv�ndaren ska kunna v�lja bland alla fr�gekategorier som opentdb tillhandah�ller. Dessutom ska man kunna v�lja sv�righetsgrad, samt hur m�nga fr�gor man vill importera. 
Anropen till API:et ska g�ras asynkront s� att GUI inte l�ses upp medan man h�mtar kategorier / fr�gor.
Felhantering m�ste fungera, och anv�ndaren ska f� veta om operationen g�tt bra eller om n�got g�tt fel (i s� fall vad som g�tt fel). Responskoder, och vad dessa inneb�r, g�r att hitta i API:ets dokumentation. T�nk �ven p� att andra fel (�n felkoder fr�n API:et) kan uppst�, t.ex om din dator inte har n�gon internetanslutning� detta ska inte krascha programmet.


Redovisning
Uppgiften ska l�sas individuellt. (Vi g�r delar gemensamt, men alla skriver sin egen kod).
Checka in din l�sning som ett NYTT repo p� Github.
L�mna in uppgiften p� ithsdistans med en kommentar med github-l�nken.
Betygskriterier


F�r godk�nt
Appen ska vara byggd med MVVM, och i huvudsak anv�nda data binding och commands.
Minimera koden i XAML-filernas code-behind.
Appen ska asynkront lagra och l�sa in �fr�gepaket� i json.
Man ska kunna l�gga till, ta bort, och konfigurera fr�gepaket (namn, tid, sv�righetsgrad).
Fr�gor ska kunna l�ggas till / tas bort via tangentbord, menyalternativ och knappar i app.
I spell�get ska alla fr�gor och svar i ett paket visas i slumpm�ssigt vald ordning.
F�rvald bet�nketid ska visas och r�kna ner.
Korrekt svar och spelaren svar ska visas, innan appen g�r vidare till n�sta fr�ga.
Efter alla fr�gor g�tt igenom visas ett resultat.
Det ska finnas ett helsk�rmsl�ge.


F�r v�l godk�nt
Man ska kunna importera fr�gor fr�n Open Trivia Database, via deras API.
Man kan v�lja kategori, sv�righetsgrad och antal fr�gor som ska importeras.
All kommunikation med API:et sker via asynkrona anrop.
Felhantering ska fungera och anv�ndaren ska informeras om status p� importen.
F�r VG st�lls �ven st�rre krav p� att sj�lvst�ndigt kunna utveckla applikationen och hitta l�sningar p� buggar och andra problem. (Det betyder inte att man inte f�r fr�ga om man k�r fast eller undrar �ver n�got, men jag kommer prioritera att hj�lpa alla att f� godk�nt i f�rsta hand).
Det �r ocks� h�gre krav p� kvalitet vad g�ller kod, UI-design och testning.