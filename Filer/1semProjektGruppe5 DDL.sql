CREATE DATABASE Projekt1semGruppe5;
GO
USE Projekt1semGruppe5

CREATE TABLE SmitteTal(
Dato DATE,
Roskilde INT,
Koebenhavn INT,
Aarhus INT,
Frederiksberg INT,
Kalundborg INT,
Middelfart INT,
Gentofte INT,
Solroed INT,
Alleroed INT,
Dragoer INT,
Helsingoerr INT,
Hilleroed INT,
Rudersdal INT,
Naestved INT,
Aalborg INT,
Vallensbaek INT,
Esbjerg INT,
Favrskov INT,
Frederikssund INT,
Furesoe INT,
Greve INT,
Holbaek INT,
Kolding INT,
Odense INT,
Silkeborg INT,
Skanderborg INT,
Vejle INT,
Aabenraa INT,
Ballerup INT,
Egedal INT,
Faxe INT,
Fredericia INT,
FaaborgMidtfyn INT,
Gladsaxe INT,
Guldborgsund INT,
Haderslev INT,
Halsnaes INT,
Herlev INT,
Herning INT,
Hjoerring INT,
Horsens INT,
Hvidovre INT,
Hoersholm INT,
Koege INT,
Lejre INT,
Lolland INT,
LyngbyTaarbaek INT,
Nordfyns INT,
Nyborg INT,
Randers INT,
Roedovre INT,
Skive INT,
Slagelse INT,
Soroe INT,
Stevns INT,
Svendborg INT,
Syddjurs INT,
Soenderborg INT,
Vejen INT,
Viborg INT,
Vordingborg INT,
Billund INT,
Broendby INT,
Frederikshavn INT,
Gribskov INT,
Holstebro INT,
Kerteminde INT,
Mariagerfjord INT,
Norddjurs INT,
Odder INT,
Odsherred INT,
Rebild INT,
RingkoebingSkjern INT,
Vesthimmerlands INT,
Glostrup INT,
HoejeTaastrup INT,
Jammerbugt INT,
Morsoe INT,
Thisted INT,
Toender INT,
Taarnby INT,
Assens INT,
Fredensborg INT,
Ishoej INT,
Lemvig INT,
Albertslund INT,
Bornholm INT,
IkastBrande INT,
Langeland INT,
Ringsted INT,
Struer INT,
Broenderslev INT,
Hedensted INT,
Varde INT,
Laesoe INT,
Aeroe INT,
Fanoe INT,
Samsoe INT,
NA INT
);

CREATE TABLE Kommuner(
KommuneID INT PRIMARY KEY NOT NULL,
KommuneNavn VARCHAR(100),
IncidensTal INT,
AntalSmittede INT
);

CREATE TABLE Branche(
BrancheID INT PRIMARY KEY NOT NULL,
BrancheKode NVARCHAR(6),
Niveau INT,
Titel NVARCHAR(100)
);

CREATE TABLE Lukning(
BrancheID INT NOT NULL,
LDatoTid DATETIME
);

CREATE TABLE Restriktion(
BrancheID INT NOT NULL,
RDatoTid DATETIME
);

ALTER TABLE Lukning
ADD FOREIGN KEY (BrancheID) REFERENCES Branche(BrancheID)

ALTER TABLE Restriktion
ADD FOREIGN KEY (BrancheID) REFERENCES Branche(BrancheID)

ALTER TABLE Lukning 
ADD KommuneID int

ALTER TABLE Restriktion 
ADD KommuneID int