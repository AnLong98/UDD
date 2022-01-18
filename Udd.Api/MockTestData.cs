using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Models;

namespace Udd.Api
{
    public static class MockTestData
    {

        public static List<JobApplicationIndexUnit>  GetMockTestData()
        {
            List<JobApplicationIndexUnit> unit = new List<JobApplicationIndexUnit>();

            unit.Add(new JobApplicationIndexUnit
            {
                Id = Guid.NewGuid(),
                ApplicantName = "Starac",
                ApplicantLastname = "Foco",
                ApplicantEducationlevel = 6,
                CvLetterContent = "Propratno pismo za starca focu",
                CvContent = "Tu knezovi nisu radi kavzi, nit su radi Turci izjelice. Vec je rada sirotinja raja koja globa davati ne moze nit trpjeti Turskoga zuluma.",
                CvFileName = "stotinuljeta.pdf",
                CvLetterFileName = "pisamce.pdf"

            }); 

            unit.Add(new JobApplicationIndexUnit
            {
                Id = Guid.NewGuid(),
                ApplicantName = "Marjan",
                ApplicantLastname = "Dusanski",
                ApplicantEducationlevel = 6,
                CvLetterContent = "Ja sam Marjan full stack developer iz Srbije i mnogo volim C# .NET i tako to bas sma pametan i za Javu spring",
                CvContent = "50k bruto.",
                CvFileName = "gamechanger96.pdf",
                CvLetterFileName = "ledubejbee.pdf"

            });

            unit.Add(new JobApplicationIndexUnit
            {
                Id = Guid.NewGuid(),
                ApplicantName = "Ksenija",
                ApplicantLastname = "Glupacko",
                CvLetterContent="Mnogo volim front end i bas sam kraljica za JS Angular i to. Timski sam igrac smaranjee.",
                ApplicantEducationlevel = 1,
                CvContent = "Svi kazu da su marsovci zeleni, u stvari su metalik roze i debeli.",
                CvFileName = "ksendza.pdf",
                CvLetterFileName = "ksendzinopismo.pdf"

            });

            unit.Add(new JobApplicationIndexUnit
            {
                Id = Guid.NewGuid(),
                ApplicantName = "Мојт",
                ApplicantLastname = "Егађо",
                ApplicantEducationlevel = 2,
                CvLetterContent="ЦВ на ћирилици за Ц# Јава и остало исто тако ради, а ради за Ангулар и React.",
                CvContent = "Генерал Мојт Егађо дао оставку због Ђоковића.",
                CvFileName = "ђенерал.pdf",
                CvLetterFileName = "самобогаивишеникога.pdf"

            });


            return unit;

        }
    }
}
