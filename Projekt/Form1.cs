using System.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Projekt
{
    public partial class Form1 : Form
    {
        // Private = Kan benyttes inde i alle metoderne i koden, ogs� void
        // autoID skal bruges senere i programmet til at tildele et ID til databasens brugere
        private int autoID = 1;
        // autoTwo skal bruges til at bladre i databasen
        private int autoTwo = 30;
        private int gangeBrugt = 0;
        //Path p� vores fil med alt vores data
        private string path = @"c:\Jeppe-Leif-Nathalie-Parking\Database.txt";
        private string appendText = "";
        public Form1() //Starter GUI
        {
            InitializeComponent();
            //Gemmer n�ste knappen
            n�steKnap.Enabled = false;
            n�steKnap.Visible = false;
            //Laver en mappe hvor jeg kan putte database textfilen ind
            Directory.CreateDirectory(@"c:\Jeppe-Leif-Nathalie-Parking");
            // Hvis ikke der er p�begynt en database, laver denne kode en ny fil p� ovenst�ende destination
            if (!File.Exists(path))
            {
                // Tilf�jer Christian M�rk til databasen
                string christianText = "Christian, M�rk, Vedb�k Strandvej 2, 2950, Vedb�k, 50251287, cm@cmis.dk"+Environment.NewLine;
                File.WriteAllText(path, christianText);
            }
        }
        private void gemKnap_Click(object sender, EventArgs e) //Metode der k�rer n�r jeg trykker p� gem knappen i programmet
        {
            // De n�ste to variabler skal bruges til at se om email eller mobilnummeret som brugeren inds�tter allerede findes i databasen
            bool telNrExists = false;
            bool emailExists = false;
            // Indhenter data fra programmet
            string telNrCheck = telBox.Text;
            string emailCheck = emailBox.Text;
            // Foreach l�kke s� den tager alle linjer som indeholder s�gekriteriet, og ikke kun den f�rste linje
            foreach (var line in File.ReadLines(path))
            {
                // Hvis en af linjerne indeholder telefon nummer eller email s�ttes deres variabel til true
                if (line.Contains(telNrCheck))
                {
                    telNrExists = true;
                }
                if (line.Contains(emailCheck))
                {
                    emailExists = true;
                }
            }
            // Tjekker om alle de n�dvendige textbokse er udfyldt, ellers gemmer den ikke informationerne
            if (forNavnBox.Text == "" || efterNavnBox.Text == "" || adresseBox.Text == "" || postnrBox.Text == "" || byBox.Text == "" || telBox.Text == "" || emailBox.Text == "")
            {
                MessageBox.Show("Udfyld venligst alle n�dvendige informationer");
            }
            // Tager h�jde for at alt indtastet i postnr kassen er tal
            else if (!postnrBox.Text.All(char.IsDigit))
            {
                MessageBox.Show("Postnummer kan kun indeholde tal");
                // Clearer boksen brugeren har lavet fejl i (dette bliver brugt mange gange nedenst�ende)
                postnrBox.Text = "";
            }
            // Tjekker om postnummeret er mere eller mindre end 4 cifre
            else if (postnrBox.Text.Length != 4)
            {
                MessageBox.Show("Postnummeret skal indeholde 4 cifre");
                postnrBox.Text = "";
            }
            // Tager h�jde for at alt indtastet i mobilnummer kassen er tal
            else if (!telBox.Text.All(char.IsDigit))
            {
                MessageBox.Show("Mobilnummer kan kun indeholde tal");
                telBox.Text = "";
            }
            // Tjekker om telefon nummeret er mere eller mindre end 8 cifre
            else if (telBox.Text.Length != 8)
            {
                MessageBox.Show("Mobilnummer skal indeholde 8 cifre (Ingen landekoder)");
                telBox.Text = "";
            }
            // Hvis der var en email eller telefonnummer som eksisterede udskriver vi en fejlmeddelelse
            else if (telNrExists == true)
            {
                MessageBox.Show("Dette telefonnummer er allerede registreret i databasen");
                telBox.Text = "";
            }
            else if (emailExists == true)
            {
                MessageBox.Show("Denne email er allerede registreret i databasen");
                emailBox.Text = "";
            }
            // Indbygget C# email validation tjek
            else if (!new EmailAddressAttribute().IsValid(emailBox.Text))
            {
                MessageBox.Show("Denne email er ikke valid");
                emailBox.Text = "";
            }
            // Else (alt info vi beh�ver er OK, og vi kan nu gemme brugeren)
            else
            {
                // Hvis der er indskrevet lejlighed nr. gemmer den brugerinformationern
                if (lejBox.Text == "")
                {
                    appendText = forNavnBox.Text + ", " + efterNavnBox.Text + ", " + adresseBox.Text + ", " + postnrBox.Text + ", " + byBox.Text + ", " + telBox.Text + ", " + emailBox.Text + Environment.NewLine;
                }
                // Gemmer brugerinformationerne hvis alle de n�dvendige felter er udfyldt
                else
                {
                    appendText = forNavnBox.Text + ", " + efterNavnBox.Text + ", " + adresseBox.Text + ", " + lejBox.Text + ", " + postnrBox.Text + ", " + byBox.Text + ", " + telBox.Text + ", " + emailBox.Text + Environment.NewLine;
                }
                // Sender informationen til textfil
                File.AppendAllText(path, appendText);
                MessageBox.Show("Dine oplysninger er nu gemt i databasen");
                //Benytter mig af nedenst�ende metode
                clear();
            }

        }
        public void clear() //Metode til at clear textboxene i applicationen efter en bruger har indtastet sine oplysninger
        {
            // Denne metode clearer alle textboksene i programmet
            forNavnBox.Text = "";
            efterNavnBox.Text = "";
            adresseBox.Text = "";
            postnrBox.Text = "";
            byBox.Text = "";
            lejBox.Text = "";
            telBox.Text = "";
            s�gBox.Text = "";
            emailBox.Text = "";
        }
        public void dataBoxOpstilling() //Metode til opstillingen af kolonnerne i databoksen
        {
            // tilf�jer tekst og colums til den hvide databox (Design ops�tning)
            dataKomplet.Columns.Add("NR", 40, HorizontalAlignment.Center);
            dataKomplet.Columns.Add("Data", 600, HorizontalAlignment.Center);
        }
        public void visKnap_Click(object sender, EventArgs e) //Metode til at udprinte de 0 - 15 personer til side 1 i databasen
        {
            // s�tter counter til 0 hver eneste gang vi bruger knappen
            int counter = 0;
            // Indl�ser alt data fra textfilen
            string[] readData = File.ReadAllLines(path);
            // S�tter autoID og autoTwo til deres originale v�rdier s� brugeren kan benytte sig af 0-15 personer knappen igen og igen
            //this betyder at variablen kan eksistere uden for void metoden, alle metoderne kan benytte sig af de to variabler uden at de skal defineres igen
            this.autoID = 1;
            this.autoTwo = 30;
            // Clearer den hvide datakasse for data s� vi kan inds�tte ny data
            dataKomplet.Clear();
            // L�kken l�ser hvor mange linjer der er i filen og giver counter +1 for hver linje
            foreach (string line in File.ReadLines(path))
            {
                counter++;
            }
            // Hvis det er f�rste gang brugeren trykker 1-15 bliver meddelelsen om antal personer vist, anden gang bliver den ikke vist
            if (gangeBrugt == 0)
            {
                //Udskriver antal brugere i databasen ved hj�lp af counter
                MessageBox.Show("Der er " + counter + " personer i databasen");
                this.gangeBrugt++;
            }
            // �ndre knappens tekst efter f�rste brug
            visKnap.Text = "Opdater";
            // Benytter metoden til opstilling af dataen
            dataBoxOpstilling();
            int dataNum = 0;
            do
            {
                // Benytter mig af nedenst�ende metode
                addDataToList(readData[dataNum]);
                // Plus 1 til dataNum
                dataNum++;
            }
            // bliver ved indtil vi har data fra 1-15
            while (dataNum < counter && dataNum < 15);
            if (counter > 15)
            {
                // Hvis der er mere end 15 brugere i databasen bliver n�ste knappen tilg�ngelig
                // Gemmer vis knappen efter den er blevet brugt og viser n�ste 15 knap for at undg� at brugeren laver fejl
                visKnap.Enabled = false; visKnap.Visible = false; n�steKnap.Enabled = true; n�steKnap.Visible = true;
            }
        }
        public void addDataToList(string Data) // Metode til at tilf�je informationen fra filen til den hvide databox
        {
            // ListViewItem (en genstand der skal puttes ind i den hvide databox)
            // eachrow laver en ny linje - Putter autoID ind fordi det er det nummer som skal st� til venstre for vores data
            ListViewItem eachrow = new ListViewItem("" + autoID);
            // Inds�tter vores data til venstre for det ID dataen har f�et
            ListViewItem.ListViewSubItem info = new ListViewItem.ListViewSubItem(eachrow, Data);
            // De to nedenst�ende linjer sender dataen til den hvide databox
            eachrow.SubItems.Add(info);
            dataKomplet.Items.Add(eachrow);
            // autoID skal +1 s� vi ikke tildeler den samme v�rdi flere gange
            // Igen 'this.autoID' fordi den skal kunne benyttes udenfor vores void metode
            this.autoID++;
        }

        private void s�gKnap_Click(object sender, EventArgs e) // Metode til n�r vi trykker p� 'S�g'
        {
            // Gemmer n�ste knap og viser 1-15 knap
            n�steKnap.Enabled = false; n�steKnap.Visible = false; visKnap.Enabled = true; visKnap.Visible = true;
            // S�tter knappens text tilbage til dens originale text
            visKnap.Text = "Vis brugere 1-15";
            // Definerer search som det brugeren har indtastet i tekstboksen
            string search = s�gBox.Text;
            // S�tter ID til 1 s� vi kan se hvor mange resultater vi f�r
            this.autoID = 1;
            // Clearer den hvide databox
            dataKomplet.Clear();
            // Foreach l�kke s� den taker alle linjer som indeholder s�gekriteriet, og ikke kun den f�rste linje
            foreach (string line in File.ReadLines(path))
            {
                // StringComparison.OrdinalIgnoreCase g�r s�gekriteriet case insensitive s� vi kan s�ge b�de med stort og sm�t
                // Hvis den indeholder s�gekriteriet:
                if (line.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    dataBoxOpstilling();
                    // Metoden fra tidligere genbrugt
                    addDataToList(line);
                }
            }
        }
        private void n�steKnap_Click(object sender, EventArgs e) //Metode til at trykke n�ste 15 personer i databasen 
        {
            //Indl�ser filens data
            string[] readData = File.ReadAllLines(path);
            // Vi benytter os igen af samme counter som tidligere den starter fra 0 igen
            int counter = 0;
            // Clearer den hvide databox
            dataKomplet.Clear();
            // Vi antager at brugeren har trykket 1-15 f�rst, s� vi s�tter ikke vores auto variabler tilbage til 1 og 30
            foreach (string line in File.ReadLines(path))
            {
                counter++;
            }
            //Design metoden
            dataBoxOpstilling();
            // S�tter dataNum til autoID v�rdi og -1 (Fordi det f�rste vi g�r er at +1 i vores do while l�kke)
            int dataNum = autoID - 1;
            do
            {
                // Metode fra tidligere
                addDataToList(readData[dataNum]);
                dataNum++;
            }
            // Forts�tter vores l�kke indtil dataNum er = autoTwo (30)
            while (dataNum < counter && dataNum < autoTwo);
            // f�jer +15 til autoTwo s� n�ste gang vi trykker k�rer lykken til 45 og s� 60 osv.
            autoTwo = autoTwo + 15;
            // Hvis der ikke er 15 personer i databasen at tilf�je giver vi brugeren en meddelelse:
            if (counter == dataNum)
            {
                MessageBox.Show("Enden af databasen");
                // Gemmer n�ste knap og viser tilbage til start knap
                n�steKnap.Enabled = false; n�steKnap.Visible = false; visKnap.Enabled = true; visKnap.Visible = true;
                //�ndre teksten p� knappen til tilbage til start n�r enden af databasen er n�et
                visKnap.Text = "Tilbage til start";
            }
        }
    }
}
