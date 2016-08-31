using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class About : WebBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            List<AdressTyp> lstAdressTyp = ctx.AdressTyps.ToList();
            List<AdressVariant> lstAdressVariant = ctx.AdressVariants.ToList();
            List<Person> lstPersons = ctx.Persons.ToList();
            List<Person_Adress> lstPerson_Adress = ctx.Person_Adresses.ToList();
            List<Adress> lstAdress = ctx.Adresses.ToList();
            List<GatuAdress> lstGatuAdress = ctx.GatuAdresses.ToList();
            List<Mail> lstMail = ctx.Mails.ToList();
            List<Telefon> lstTelefon = ctx.Telefons.ToList();

            foreach (Person person in lstPersons.OrderBy(o=> o.PersonNummer))
            {
                TreeNode TnPerson = new TreeNode(person.PersonNummer + "; " + person.Efternamn + "; " + person.MellanNamn + "; " + person.Efternamn);
                var lstTnGatuAdress = (from a in lstGatuAdress
                                                  join b in lstAdress on a.Adress_FKID equals b.Id
                                                  join c in lstAdressVariant on b.AdressVariant_FKID equals c.Id
                                                  join d in lstPerson_Adress on b.Id equals d.Adress_FKID
                                                  where d.Person_FKID == person.Id
                                                  select new { Variant = c, Adress = a }).ToList();

                foreach (var tnGatuAdress in lstTnGatuAdress)
                {
                    TreeNode personAdressVariant = new TreeNode(tnGatuAdress.Variant.Variant);
                    List<string> lstAdressRader = new List<string>()
                    {
                        tnGatuAdress.Adress.AdressRad1,
                        tnGatuAdress.Adress.AdressRad2,
                        tnGatuAdress.Adress.AdressRad3,
                        tnGatuAdress.Adress.AdressRad4,
                        tnGatuAdress.Adress.AdressRad5,
                        tnGatuAdress.Adress.Postnummer.ToString(),
                        tnGatuAdress.Adress.Stad,
                        tnGatuAdress.Adress.Land,
                    };
                    lstAdressRader.ForEach(f => personAdressVariant.ChildNodes.Add(new TreeNode(f)));
                    TnPerson.ChildNodes.Add(personAdressVariant);
                }

                var lstTnMail = (from a in lstMail
                                            join b in lstAdress on a.Adress_FKID equals b.Id
                                            join c in lstAdressVariant on b.AdressVariant_FKID equals c.Id
                                            join d in lstPerson_Adress on b.Id equals d.Adress_FKID
                                            where d.Person_FKID == person.Id
                                            select new { Variant = c, Adress = a }).ToList();

                foreach (var tnMail in lstTnMail)
                {
                    TreeNode mailAdressVariant = new TreeNode(tnMail.Variant.Variant);

                    mailAdressVariant.ChildNodes.Add(new TreeNode(tnMail.Adress.MailAdress));
                    TnPerson.ChildNodes.Add(mailAdressVariant);
                }

                var lstTnTelefon = (from a in lstTelefon
                                               join b in lstAdress on a.Adress_FKID equals b.Id
                                               join c in lstAdressVariant on b.AdressVariant_FKID equals c.Id
                                               join d in lstPerson_Adress on b.Id equals d.Adress_FKID
                                               where d.Person_FKID == person.Id
                                               select new { Variant = c, Adress = a }).ToList();

                foreach (var tnTelefon in lstTnTelefon)
                {
                    TreeNode telefonAdressVariant = new TreeNode(tnTelefon.Variant.Variant);

                    telefonAdressVariant.ChildNodes.Add(new TreeNode("0" + tnTelefon.Adress.TelefonNummer));
                    TnPerson.ChildNodes.Add(telefonAdressVariant);
                }

                tvData.Nodes.Add(TnPerson);
            }
            tvData.CollapseAll();
        }
    }
}