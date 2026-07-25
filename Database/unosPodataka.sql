USE [Biblioteka];
GO

DELETE FROM Pozajmica;
DELETE FROM Clanarina;
DELETE FROM Clan;
DELETE FROM PrimerakGradje;
DELETE FROM Bibliotekar;
DELETE FROM Ogranak;
DELETE FROM Naselje;
DELETE FROM GradjaAutor;
DELETE FROM OcenaProcitaneGradje;
DELETE FROM Gradja;
DELETE FROM IzdavanjeGradje;
DELETE FROM Autor;
DELETE FROM Nalog;
GO

CREATE OR ALTER PROCEDURE [dbo].[spUnesiPrimerakGradje](@inGradjaFK INT, @inOgranakFK INT, @inSignatura NVARCHAR(80)) AS
BEGIN TRY
    DECLARE @rbrNaselje INT = -1, @rbrOgranak INT = -1;
	  
	SELECT @rbrNaselje = n.NaseljeID, @rbrOgranak = og.RbrUOkviruNaselja
	FROM Ogranak og INNER JOIN Naselje n ON og.NaseljeFK = n.NaseljeID
	WHERE og.OgranakID = @inOgranakFK;

	DECLARE @rbrPrimerka INT = -1;

	SELECT @rbrPrimerka = ISNULL(MAX(pr.RbrUOkviruOgranka) + 1, 1)
	FROM PrimerakGradje AS pr
	WHERE pr.OgranakFK = @inOgranakFK;

	DECLARE @inventarniBroj CHAR(11) = CONCAT(
		FORMAT(@rbrNaselje, '000'),
		FORMAT(@rbrOgranak, '000'),
		FORMAT(@rbrPrimerka, '00000')
	);
    
    INSERT INTO PrimerakGradje(GradjaFK, OgranakFK, RbrUOkviruOgranka, Signatura, InventarniBroj, Status)
	SELECT @inGradjaFK, @inOgranakFK, @rbrPrimerka, @inSignatura, @inventarniBroj, 'slobodan';
END TRY
BEGIN CATCH
	PRINT 'DOŠLO JE DO POJAVE GREŠKE!';
	PRINT ERROR_MESSAGE() + ' (ERROR_NUMBER: ' + CAST(ERROR_NUMBER() AS VARCHAR) + ', STATE: ' + CAST(ERROR_STATE() AS VARCHAR) + ')';
	ROLLBACK TRANSACTION;
END CATCH
GO

CREATE OR ALTER TRIGGER trg_StatusPrimerka ON [Pozajmica]
AFTER INSERT
AS
	UPDATE pg
	SET pg.Status = 'zauzet'
	FROM PrimerakGradje pg INNER JOIN Inserted i
		 ON pg.GradjaFK = i.PrimerakGradje_GradjaFK AND
		    pg.OgranakFK = i.PrimerakGradje_OgranakFK AND
			pg.RbrUOkviruOgranka = i.PrimerakGradjeFK
	WHERE i.DatumRazduzenja IS NULL;
GO

-- NASELJA
INSERT INTO Naselje(NaseljeID, Naziv) VALUES
(1, N'Beograd'),
(2, N'Novi Sad');

-- OGRANCI
INSERT INTO Ogranak(OgranakID, Naziv, Adresa, NaseljeFK, RbrUOkviruNaselja) VALUES
(1, N'Marija Ilić Agapova', N'Narodnih heroja 42, blok 37', 1, 1),
(2, N'Meša Selimović', N'Jurija Gagarina 81, blok 70', 1, 2),
(3, N'Danilo Kiš', N'Narodnog fronta 47', 2, 1);

-- IZDAVANJA, AUTORI, GRADJE, GRADJA_AUTORI i PRIMERCI
INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (1, N'Beograd', N'Zavod za udžbenike', 2024);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (1, N'Sofokle');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (1, N'Antigona.png', N'Antigona', N'5. preštampano izdanje – 2024. god. Sofokle je ovu tragediju napisao 442. ili 441. godine pre nove ere. U njoj se Antigona suprotstavlja Kreontu, odlučivši da prenebregne njegovu zabranu da sahrani brata Polinika, koji je udario na rodnu Tebu. Žena ustaje protiv muškarca i protiv vladara. Usuđuje se, u ime večnih, božanskih zakona, da prekrši ljudske zakone sile i vlasti,zbog čega biva kažnjena. Ali još surovije će biti kažnjen njen sudija, čime se zatvara krug hibrisa, čovekovog prekoračenja određenih granica i njegovog zalaženja u božje atare, nakon čega sledi neminovna kazna. Mit koji je Sofokle (496‒406. god. pre n. e.) dramatizovao u Antigoni pripada krugu tebanskih legendi. Izdavanjem zapovesti da Polinik, Antigonin brat, ostane nesahranjen, Kreont, novi vladar Tebe, sledi atinski zakon koji ne dozvoljava pokop neprijatelja države. Porodica preminulog ima, međutim,obavezu prema Poliniku: mrtvi moraju da budu sahranjeni, makar simbolično, kako to Antigona i čini, pokrivanjem tela tankim slojem zemlje. Izvršavajući obavezu prema bogovima kojima pripada duša poginulog, ona krši Kreontovu zapovest. Grčka tragedija doživljava procvat u petom veku pre nove ere ‒ tada stvaraju Eshil, Sofokle i Euripid ‒ a nastala je, po svemu sudeći,vek ranije, i to, po tumačenju i svedočenju Aristotela, iz ditiramba, horskih pesama pevanih u slavu boga Dionisa, razvojem pratećih „improvizacijaˮ, odnosno pripovednih i dijaloških delova i odbacivanjem šaljivih govora.', N'978-86-17-21116-3', N'821.14''02-21; 821.14''02-21.09 Sofokle; 929:821.14''02 Sofokle', 1);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (1, 1);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 1, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 1, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (2, N'Beograd', N'Book', 2020);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (2, N'Šekspir Vilijam');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (2, N'Hamlet.jpg', N'Hamlet', null, N'978-86-7572-163-5', N'821.111-21; 821.111-21.09 Šekspir V.; 929:821.111 Šekspir V.', 2);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (2, 2);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 2, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (3, N'Beograd [etc.]', N'Kosmos izdavaštvo [etc.]', 2021);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (3, N'Gete Johan Volfgang');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (3, N'Faust.jpg', N'Faust', N'Geteovo remek-djelo i jedno od najvećih dostignuća njemačke književnosti, Faust  je evokacija priče o legendarnom njemačkom alhemičaru. U pitanju je jedan od temeljnih mitova zapadnog svijeta. Polazeći od klasične priče o čarobnjaku, alhemičaru, koji u zamjenu za tajanstvena znanja prodaje dušu đavolu, Gete prevazilazi vremenski jaz između srednjovjekovne priče i čovjeka evropske modernosti koji je u potrazi za životnim zadovoljstvima, ali takođe i za novim spoznajama u vremenu koje još živi ideale Prosvjetiteljstva. Geteov je Faust monumentalni lik, smioni čovjek koji izaziva lukavog Mefistofela.', N'978-86-6369-107-0', N'821.112.2-12', 3);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (3, 3);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 3, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 3, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 3, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (4, N'Beograd', N'LOM', 2022);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (4, N'Selindžer Džerom Dejvid');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (4, N'Lovac u žitu.jpg', N'Lovac u žitu', N'Lovac u žitu Dž. D. Selindžera je kultna knjiga jedne generacije koja i dalje ne gubi na aktuelnosti. Mladi junak u svojim beskrajnim šetnjama Njujorkom promišlja moral, odrastanje i život. Lovac u žitu je kritika i glas posleratne Amerike. Selindžeru je trebalo gotovo deset godina da napiše ovo delo, koje je izrazilo egzistencijalne strepnje mlade generacije u posleratnoj Americi, kao i bunt protiv licemerja i laži u svetu njegovog junaka, Holdena Kolfilda, koji je ubrzo postao mitska ličnost, alter-ego za mnoge generacije izgubljenih tinejdžera u Americi i širom sveta, tako da je značaj samog romana daleko prevazišao granice književnosti.Lovac u žitu" je zaista najčitaniji roman na planeti, koji je kulturnom nasleđu čovečanstva podario jednog sasvim drugačijeg junaka - mladog lutalicu i uličnog filozofa Holdena Kolfilda.', N'978-86-7958-031-3', N'821.111(73)-31; 929:821.111(73) Selindžer Dž. D.', 4);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (4, 4);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 4, @inOgranakFK = 1, @inSignatura = '0101-X-II-196480';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 4, @inOgranakFK = 2, @inSignatura = '022- 821.111(73)-31';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 4, @inOgranakFK = 3, @inSignatura = '030- 821.111(73)-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (5, N'Beograd', N'Laguna', 2022);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (5, N'Tolstoj Lav Nikolajevič');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (5, N'Rat i mir , Tom 1.png', N'Rat i mir , Tom 1', N'"Rat i mir" je filozofski, istorijski i epski roman-reka u kojem Lav Tolstoj analizira složen proces istorijske stvarnosti, društvene dinamike i ljudskog ponašanja. Ideali i smisao ljudskog postojanja centralna su tema ovog remek-dela. Radnja se odvija tokom Napoleonove invazije na Rusiju 1812. godine i prati sudbine troje junaka: Pjera Bezuhova, vanbračnog sina jednog plemića, koji se bori za svoje nasledstvo i duhovno ispunjenje, kneza Andreja Bolkonskog koji ostavlja svoju porodicu da bi ratovao protiv Napoleona, i Nataše Rostove, prelepe mlade grofice koja intrigira obojicu. Kako Napoleonova vojska napreduje, tako nam Tolstoj u sve širim potezima oslikava pozadinu istorijskih dešavanja, pa se na poleđini ove impresivne freske pojavljuju seljaci, vojnici i građanstvo u svojoj borbi sa problemima doba u kojem žive. Što se više pred nama razvija ova fascinantna saga, tako njeni junaci prevazilaze svoje književno obličje i postaju sve dirljivije i čitaocu bliže figure.', N'978-86-521-4508-9', N'821.161.1-31; 821.161.1-31.09 Tolstoj L. N.', 5);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (5, 5);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 5, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 5, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 5, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (6, N'Rat i mir , Tom 2.png', N'Rat i mir , Tom 2', N'U Tolstojevoj istorijskoj epopeji prepliću se intiman i društveni život dve porodice, Rostovih i Bolkonskih, a sudbine troje glavnih junaka, Pjera, Nataše i Andreja duboko su povezane sa nacionalnom istorijom koja je paralelno prikazana. Balovi i salonska okupljanja smenjuju se naizmenično sa opisima ratnih saveta, mahinacijama državnika i generala, scenama žestokih bitaka i onima iz svakodnevnog života. Rat i mir&nbsp;maestralno spaja sve velike teme: sukob i ljubav, rođenje i smrt, slobodnu volju i sudbinu – s nezaboravnim scenama devetnaestovekovne Rusije. Veličanstvena epska slika ljudskog života prikazana je u svoj svojoj veličini i nesavršenosti. Tolstojeva duboka analiza ljudske duše fascinira čitaoce više od vek i po zbog izuzetne misaone i stvaralačke snage koju do sada niko nije prevazišao.', N'978-86-521-4509-6', N'821.161.1-31; 929:821.161.1 Tolstoj L. N.', 5);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (6, 5);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 6, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 6, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 6, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (6, N'Beograd', N'Zavod za udžbenike', 2020);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (6, N'Puškin Aleksandar Sergejevič');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (7, N'Evgenije Onjegin.png', N'Evgenije Onjegin', N'Aleksandar Puškin je u Evgeniju Onjeginu dao umetnički presek ruskog društva u jednom od najzanimljivijih perioda njegovog razvoja. U ovom remek-delu ruske književnosti zahvaćen je čitav ruski život dvadesetih godina XIX veka: od najviših plemićkih krugova do seljačkog staleža, od visokoobrazovanog intelektualca do čestite dadilje. Onjegin i Lenski predstavljaju dva tipa mladih obrazovanih plemića čija je tragična sudbina posledica raskoraka između njihovih želja i sposobnosti i života koji vode. Kod Lenskog je uzrok razočaranja i nesreće idealizovanje stvarnosti koja nikad nije bila onakva kakvom ju je pesnik zamišljao dok je kod Onjegina taj sukob zasnovan na dubokim unutrašnjim protivrečnostima. Lik Onjegina poslužio je kao uzor čitavoj galeriji tipova, takozvanih „suvišnih ljudi“, kao što su Ljermontovljev Pečorin ili Turgenjevljev Ruđin. S druge strane, jedina koja je u Puškinovom romanu svoju nesreću primila dostojanstveno, ne gubeći ni u jednom trenutku ravnotežu i dodir sa običnim narodom, bila je Tatjana, prva u nizu nezaboravnih junakinja moderne ruske književnosti. Delo je imalo mnogo filmskih adaptacija, jedna od novijih je i englesko-američki film iz 1999. godine sa Ralfom Fajnsom i Liv Tajler u glavnim ulogama.', N'978-86-17-20389-2', N'821.161.1-1; 821.161.1-1.09 Puškin A. S.(082); 929:821.161.1 Puškin A. S.', 6);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (7, 6);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 7, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 7, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 7, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (7, N'Beograd', N'Laguna [itd.]', 2022);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (7, N'Dostojevski Fjodor Mihajlovič');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (8, N'Zli dusi , Knj. 1.jpg', N'Zli dusi , Knj. 1', N'Budite sigurni da svi oni koji svoj narod prestaju da razumeju i gube vezu s njim, odmah i u istoj meri gube i veru očeva, postaju ili ateisti ili ravnodušni ljudi. Istinu govorim! To je činjenica čija se tačnost stalno dokazuje. Eto zbog čega smo vi i mi, svi zajedno sada, ili gadni ateisti ili ravnodušan razvratan ološ, i ništa drugo! U jedan provincijski gradić vraća se Nikolaj Stavrogin, plemić i bivši oficir, harizmatičan čovek koji ne uspeva nigde da se skrasi niti veže za neku ideju, što ga čini nihilistom. On očarava sve koji ga upoznaju, i tako privlači fanatične a neželjene sledbenike, podjednako žene i muškarce, koji postaju mahniti pod uticajem njegovih ideja. Tokom mesec dana njegovog boravka nižu se dramatični događaji, jer sve junake kao da zaposednu demoni različitih ideologija: ateističkog socijalizma, revolucionarnog nihilizma, verskog sujeverja.Dostojevski je predosetio opasnosti radikalnih ideologija koje su potresale XX vek, i svoje sumnje i strepnje genijalno ovaplotio u svom romanu.„Dugo smo verovali da je Marks bio prorok XX veka. Otkrićemo da je pravi prorok bio Dostojevski. Prorokovao je režim velikih inkvizitora i trijumf moći nad pravdom.“– Alber Kami„Dostojevski je jedini psiholog od koga sam imao šta da naučim: to je jedan od najsrećnijih zgoditaka u mom životu, još srećniji od otkrića Stendala.“– Fridrih Niče„Egzistencijalisti bi trebalo da počnu sa Zločinom i kaznom, istoričari društva sa Zlim dusima, a oni u potrazi za duhovnim aspektom sa Braćom Karamazov.“', N'978-86-521-4511-9', N'821.161.1-31; 821.161.1-31.09 Dostojevski F. M.', 7);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (8, 7);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 8, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 8, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 8, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (9, N'Zli dusi , Knj. 2.jpg', N'Zli dusi , Knj. 2', N'Metafora katastrofalnih posledica političkog i moralnog nihilizma. Budite sigurni da svi oni koji svoj narod prestaju da razumeju i gube vezu s njim, odmah i u istoj meri gube i veru očeva, postaju ili ateisti ili ravnodušni ljudi. Istinu govorim! To je činjenica čija se tačnost stalno dokazuje. Eto zbog čega smo vi i mi, svi zajedno sada, ili gadni ateisti ili ravnodušan razvratan ološ, i ništa drugo! U jedan provincijski gradić vraća se Nikolaj Stavrogin, plemić i bivši oficir, harizmatičan čovek koji ne uspeva nigde da se skrasi niti veže za neku ideju, što ga čini nihilistom. On očarava sve koji ga upoznaju, i tako privlači fanatične a neželjene sledbenike, podjednako žene i muškarce, koji postaju mahniti pod uticajem njegovih ideja. Tokom mesec dana njegovog boravka nižu se dramatični događaji, jer sve junake kao da zaposednu demoni različitih ideologija: ateističkog socijalizma, revolucionarnog nihilizma, verskog sujeverja.Dostojevski je predosetio opasnosti radikalnih ideologija koje su potresale XX vek, i svoje sumnje i strepnje genijalno ovaplotio u svom romanu.„Dugo smo verovali da je Marks bio prorok XX veka. Otkrićemo da je pravi prorok bio Dostojevski. Prorokovao je režim velikih inkvizitora i trijumf moći nad pravdom.“– Alber Kami„Dostojevski je jedini psiholog od koga sam imao šta da naučim: to je jedan od najsrećnijih zgoditaka u mom životu, još srećniji od otkrića Stendala.“– Fridrih Niče„Egzistencijalisti bi trebalo da počnu sa Zločinom i kaznom, istoričari društva sa Zlim dusima, a oni u potrazi za duhovnim aspektom sa Braćom Karamazov.“', N'978-86-521-4512-6', N'821.161.1-31; 929:821.161.1 Dostojevski F. M.', 7);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (9, 7);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 9, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 9, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 9, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (8, N'Beograd', N'Službeni glasnik', 2022);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (8, N'Kafka Franc');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (10, N'Proces.png', N'Proces', N'Slobodan sam i zato sam izgubljen. Prvi put kod nas imamo sve što je, do poslednjeg fragmenta, u svom kratkom životu, u kreativnoj prozi napisao Franc Kafka, jedan od gorostasa svetske književnosti. U ovoj kolekciji je i zbirka svih njegovih crteža, među kojima su i novootkriveni. Najzad, kolekciji je priložen i jedini autentičan razgovor s piscem, kao i sabrani zapisi Valtera Benjamina o Kafki, koji su od njihovog nastanka do danas ostali najpodsticajniji uvidi u nerešivu i bolnu zagonetku o moćnom i univerzalnom smislu koji je praški književnik utkao, kao malo ko drugi, usvoje nezaboravno delo. Orson Vels je 1962. godine snimio istoimeni film. Poznata je i filmska verzija iz 1993. godine (Kajl Maklaklan, Entoni Hopkins).', N'978-86-519-2535-4', N'821.112.2(436)-31; 821.112.2(436)-31.09 Kafka F.; 929:821.112.2(436) Kafka F.', 8);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (10, 8);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 10, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 10, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 10, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO Autor(AutorID, ImePrezime) VALUES (9, N'Čehov Anton Pavlovič');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (11, N'Ujka Vanja.png', N'Ujka Vanja', N'Zaplet “Ujka Vanje” - A. P. Čehov (1860-1904) - sazdan je na jednostavnom obrascu posete: profesor Serbrjakov i njegova žena Jelena došli su na imanje profesorove prve žene s namerom da one koji tamo žive nagovore da prodaju imanje i time reše materijalni problem u kojem su se našli posle njegovog odlaska u penziju. Sem toga, na drugom planu taj njihov dolazak proizvodi tri erotske intrige: Vojnicki voli Jelenu, ali ne i ona njega; Sonja voli Astorova, ali ne i on nju; Astorov voli Jelenu koja, iako nije ravnodušna prema njemu, ipak ne može da mu uzvrati ljubavlju. Pošto u drugom i trećem činu propadaju i ljubavne želje i snovi a takođe i profesorov plan da proda imanje, u četvrtom činu posetioci mogu još samo da se pozdrave i odu, a domaći nastavljaju da žive kao ranije. Tako je na oba plana zapleta “Ujka Vanje“- erotskom i materijalnom - ishod neuspeh i propast.', N'978-86-17-21155-2', N'821.161.1-2; 821.161.1-2.09 Čehov A. P.', 1);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (11, 9);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 11, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 11, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 11, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (9, N'Beograd', N'Laguna [itd.]', 2019);
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (12, N'Zapisi iz podzemlja.jpg', N'Zapisi iz podzemlja', N'Jedan od prvih egzistencijalističkih romana u istoriji svetske književnosti, Zapisi iz podzemlja predstavljaju zabeleške gorkog, izolovanog i nepouzdanog pripovedača. Zasnovan na jedinstvu dva suprotstavljena kompleksa, kompleksa inferiornosti i kompleksa superiornosti, čovek iz podzemlja je u socijalnom smislu neprihvaćen, neostvaren, siromašan, ali je, kako se njemu čini, u duhovnom i intelektualnom pogledu superioran u odnosu na ostale. Istovremeno labilan i osion, on spada u red najomiljenijih antijunaka među svim likovima Fjodora Dostojevskog. „Vrhunac autorovog stvaralaštva.“ Andre Žid', N'978-86-521-3554-7', N'821.161.1-31; 821.161.1-31.09 Dostojevski F. M.; 929:821.161.1 Dostojevski F. M.', 9);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (12, 7);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 12, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 12, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 12, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (10, N'Beograd', N'Laguna', 2021);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (11, N'Orvel Džordž');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (13, N'Životinjska farma.jpg', N'Životinjska farma', N'Kultna knjiga XX veka. „Slabi ili jaki, pametni ili priprosti, svi smo mi braća. Nijedna životinja nikad ne sme ubiti neku drugu. Sve životinje su jednake. A sada, drugovi, ispričaću vam šta sam prošle noći sanjao. Ne mogu vam opisati taj san. Bio je to san o tome kako će zemlja izgledati pošto Čovek sa nje iščezne. Ali podsetio me je na nešto što sam odavno već zaboravio.“ Povod za nastanak Životinjske farme nalazio se u Orvelovoj analizi posledica Ruske revolucije koja je prerasla u totalitarni režim i diktaturu stvorenu oko kulta ličnosti, kao i u njegovom iskustvu stečenom tokom Španskog građanskog rata. Iako je javnost često insistirala na momentu kritike sovjetske vlasti, Orvel je uvek naglašavao da se Životinjska farma iako prvenstveno satira o Ruskoj revoluciji, odnosi na svaku nasilnu revoluciju koju predvode nemarni ljudi gladni vlasti. „Hteo sam da naravoučenije bude da revolucije donose radikalno poboljšanje samo kada su mase budne i znaju kako da zbace svoje vođe čim ovi obave svoj posao. Trebalo je da prekretnica u priči bude trenutak kada svinje zadrže mleko i jabuke za sebe“, isticao je Orvel razočaran što je čitaocima možda promakao ovaj ključni momenat teksta. I pored piščeve bojazni, ova alegorijska antiutopija odmah je privukla veliku pažnju. Danas se smatra jednim od najboljih romana XX veka, postala je neizostavna lektira svakog savremenog čitaoca, a 1996. godine dobila je nagradu Hugo. Danas je možda više nego ikad pre potrebno da ovo delo čitamo i razumemo. „Sve životinje su jednake, ali neke životinje su jednakije od drugih.“', N'978-86-521-4264-4', N'821.111-313.1', 10);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (13, 11);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 13, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 13, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 13, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (11, N'Beograd', N'Vulkan izdavaštvo', 2023);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (12, N'Servantes Saavedra Migel de');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (14, N'Don Kihot.jpg', N'Don Kihot', N'U središtu priče je plemeniti, mada pomalo ekscentrični vitez Don Kihot, čija strast prema viteškim romanima prelazi granice stvarnosti. Odlučan da sledi svoje ideale, Don Kihot kreće na epsko putovanje sa svojim vernim skudnikom, Sančom Pansom, u potrazi za avanturama koje će mu doneti slavu i priznanje. Međutim, Don Kihotova maštovitost često ga dovodi u nevolje, jer nije u stanju da razlikuje stvarnost od fikcije. Njegova najpoznatija borba, sa vetrenjačama koje on zamišlja kao divove, postaje simbol njegove nepokolebljive hrabrosti i nepokolebljivosti u odbrani svojih ideala. Suprotno Don Kihotovoj idealističkoj prirodi stoji Sančo Pansa, njegov verni pratilac, koji predstavlja zdrav razum i praktičnost. Njihov odnos, ispunjen humorom i dubinom, ogleda se u stalnom sukobu između maštovitosti i stvarnosti, između ideala i pragmatizma. “Don Kihot” nije samo priča o jednom vitezu, već duboko istraživanje ljudske prirode, snova i ideala. Servantesova genijalnost leži u tome što je ovaj klasik uspeo da nasmeje, ali istovremeno i da nas natera na razmišljanje o suštini ljudskog bića.', N'978-86-10-04999-2', N'821.134.2-31; 821.134.2-31.09 Servantes Saavedra M. de; 929:821.134.2 Servantes Saavedra M. de', 11);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (14, 12);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 14, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 14, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 14, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';


INSERT INTO IzdavanjeGradje(IzdavanjeID, NaseljeIzdavanja, NazivIzdavaca, GodinaIzdavanja) VALUES (12, N'Beograd', N'Dereta', 2016);
INSERT INTO Autor(AutorID, ImePrezime) VALUES (13, N'Dante Aligijeri');
INSERT INTO Gradja(GradjaID, NaslovnaStranaPath, Naslov, Opis, ISBN, UDK, IzdavanjeFK) VALUES (15, N'Božanstvena komedija.jpg', N'Božanstvena komedija', N'Danteova Božanstvena komedijakoja se sastoji od tri dela - Pakao, Čistilište i Raj - ta „sveta poema“ koja je obeležila italijanski XIV vek, prerasla je svoje vreme i postala blago, po mnogima do sada neprevaziđeno, sveg čovečanstva. U ovom delu pesnik silazi u tamu ljudskog greha i stiže do svetlosti oslobođenja od njega, a na svome putu kroz tri zagrobna carstva on obuhvata nebo i zemlju, vreme i večnost, božansko i ljudsko. To je traganje ljudske duše za apsolutnom srećom, slobodom, mirom i konačnim prosvetljenjem. Delo Dantea Aligijerija (1265-1321), jednog od najvećih svetskih pisaca, nastaje u onom periodu Srednjeg veka kada se već pomalja nova i otvorenija civilizacija, ali još opstaju krute dogmatske i skolastičke šeme. Na mesto pisaca doktrinarne književnosti dolaze novi pesnici. Dante je jedan od najznačajnijih tvoraca tog novog talasa kojim se završava Srednji vek u književnosti i otvara prostor renesanse.', N'978-86-6457-106-7', N'821.131.1-13; 821.131.1-13.09 Dante A.; 929:821.131.1 Dante A.', 12);
INSERT INTO GradjaAutor(GradjaFK, AutorFK) VALUES (15, 13);

-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 15, @inOgranakFK = 1, @inSignatura = '0101-X-II-196864';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 15, @inOgranakFK = 2, @inSignatura = '0206- 821.111-21';
-- ================================
EXEC spUnesiPrimerakGradje @inGradjaFK = 15, @inOgranakFK = 3, @inSignatura = '0320- 821.161.1-31';
GO

-- Nalozi

SET IDENTITY_INSERT Nalog ON;
INSERT INTO Nalog(NalogID, Lozinka, Uloga) VALUES
(1, N'passadmin1', N'Admin_Bibliotekar'),
(2, N'passadmin2', N'Admin_Bibliotekar'),
(3, N'passadmin3', N'Admin_Bibliotekar'),
(4, N'pass', N'Korisnik_Clan');
SET IDENTITY_INSERT Nalog OFF;

-- Clanovi

SET IDENTITY_INSERT Clan ON;
INSERT INTO Clan(ClanID, JCB, DatumUclanjenja, BrLicneKarte, ImePrezime, DatumRodjenja, ImeRoditelja, AdresaStanovanja, Zanimanje, KontaktTelefon, KontaktMejl, KorisnickiNalogFK) VALUES
(1, N'1/2024', CAST('2024-01-01' AS DATE), N'548392170', N'Adam Marković', CAST('1992-05-04' AS DATE), N'Marko', N'Beograd, 11070, Gandijeva 139/11', N'Kristalografista', N'+381615530491', N'adammarkovic92@gmail.com', NULL),
(2, N'2/2024', CAST('2024-01-02' AS DATE), N'983641052', N'Bogdan Ivković', CAST('1993-06-05' AS DATE), N'Jovan', N'Beograd, 11070, Gandijeva 139/12', N'Kriptolog', N'+381629202985', N'bogdanivkovic93@gmail.com', NULL),
(3, N'3/2024', CAST('2024-01-03' AS DATE), N'746109384', N'Vojan Stojković', CAST('1994-07-06' AS DATE), N'Dejan', N'Beograd, 11070, Gandijeva 139/13', N'Teoretski fizičar', N'+381630913475', N'vojanstojkovic94@gmail.com', NULL),
(4, N'4/2024', CAST('2024-01-04' AS DATE), N'215870649', N'Goran Petrović', CAST('1995-08-07' AS DATE), N'Marija', N'Beograd, 11070, Gandijeva 139/21', N'Hemijski tehničar', N'+381649810034', N'goranpetrovic95@gmail.com', 4),
(5, N'5/2024', CAST('2024-01-05' AS DATE), N'610257893', N'Dejan Matić', CAST('1996-09-08' AS DATE), N'Ana', N'Beograd, 11070, Gandijeva 139/22', N'Biofizičar', N'+38165390123', N'dejanmatic96@gmail.com', NULL);
SET IDENTITY_INSERT Clan OFF;
GO

-- Clanarine

DECLARE @datum DATE = CAST(DATEADD(dd, 0, GETDATE()) AS DATE);
DECLARE @datum_5danaunapred DATE = CAST(DATEADD(dd, 5, @datum) AS DATE);

SET IDENTITY_INSERT Clanarina ON;
INSERT INTO Clanarina(ClanFK, Rbr, DatumPocetka, DatumZavrsetka, Cena) VALUES
(1, 1, CAST(DATEADD(yy, -1, @datum_5danaunapred) AS DATE), CAST(@datum_5danaunapred AS DATE), 1000),
(3, 1, CAST(DATEADD(yy, -1, @datum_5danaunapred) AS DATE), CAST(@datum_5danaunapred AS DATE), 1000);
SET IDENTITY_INSERT Clanarina OFF;

-- Pozajmice
			
SET IDENTITY_INSERT Pozajmica ON;
INSERT INTO Pozajmica(Clanarina_ClanFK, ClanarinaFK, Rbr, PrimerakGradje_GradjaFK, PrimerakGradje_OgranakFK, PrimerakGradjeFK, DatumPocetka, RokRazduzenja, DatumRazduzenja) VALUES
(1, 1, 1, 3, 1, 3, CAST(DATEADD(dd, -30, @datum_5danaunapred) AS DATE), CAST(DATEADD(dd, -10, @datum_5danaunapred) AS DATE), NULL),
(1, 1, 2, 4, 1, 4, CAST(DATEADD(dd, -20, @datum_5danaunapred) AS DATE), @datum_5danaunapred, CAST(DATEADD(dd, -7, @datum_5danaunapred) AS DATE)),
(1, 1, 3, 7, 1, 7, CAST(DATEADD(dd, -20, @datum_5danaunapred) AS DATE), @datum_5danaunapred, CAST(DATEADD(dd, -7, @datum_5danaunapred) AS DATE)),
(1, 1, 4, 8, 1, 8, CAST(DATEADD(dd, -20, @datum_5danaunapred) AS DATE), @datum_5danaunapred, CAST(DATEADD(dd, -7, @datum_5danaunapred) AS DATE)),
(1, 1, 5, 9, 1, 9, CAST(DATEADD(dd, -20, @datum_5danaunapred) AS DATE), @datum_5danaunapred, CAST(DATEADD(dd, -7, @datum_5danaunapred) AS DATE)),
(1, 1, 6, 10, 1, 10, @datum, CAST(DATEADD(dd, 20, @datum) AS DATE), NULL),
(1, 1, 7, 11, 2, 10, CAST(DATEADD(dd, -20, @datum_5danaunapred) AS DATE), @datum_5danaunapred, NULL),
(1, 1, 8, 12, 1, 12, CAST(DATEADD(dd, -23, @datum_5danaunapred) AS DATE), CAST(DATEADD(dd, -3, @datum_5danaunapred) AS DATE), NULL);

INSERT INTO Pozajmica(Clanarina_ClanFK, ClanarinaFK, Rbr, PrimerakGradje_GradjaFK, PrimerakGradje_OgranakFK, PrimerakGradjeFK, DatumPocetka, RokRazduzenja, DatumRazduzenja) VALUES
(3, 1, 1, 4, 1, 4, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 2, 5, 1, 5, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 3, 6, 1, 6, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 4, 7, 1, 7, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 5, 8, 1, 8, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 6, 9, 1, 9, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 7, 11, 1, 11, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 8, 13, 1, 13, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 9, 14, 1, 14, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL),
(3, 1, 10, 15, 1, 15, CAST(DATEADD(dd, -10, @datum) AS DATE), CAST(DATEADD(dd, 10, @datum) AS DATE), NULL);
SET IDENTITY_INSERT Pozajmica OFF;

-- Bibliotekari

INSERT INTO Bibliotekar(BibliotekarID, JBB, ImePrezime, Email, OgranakFK, AdminNalogFK) VALUES
(1, N'1/2011', N'Aleksa Janković', N'aleksa.jankovic@biblioteka.rs', 1, 1),
(2, N'2/2011', N'Bojan Stojović', N'bojan.stojovic@biblioteka.rs', 2, 2),
(3, N'3/2011', N'Veljko Ivković', N'veljko.ivkovic@biblioteka.rs', 3, 3);
GO

DROP PROCEDURE spUnesiPrimerakGradje;
GO

DROP TRIGGER trg_StatusPrimerka;
GO