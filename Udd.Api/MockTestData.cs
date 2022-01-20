using Nest;
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
                CvContent = "CV za starca focu",
                CvLetterContent = @"Are you searching for a software engineer with a proven ability to develop high-performance applications and technical innovations? If so, please consider my enclosed resume.Since 2015,
                I have served as a software engineer for Action Company, where I have been repeatedly recognized for developing innovative solutions for multimillion - dollar, globally deployed software and systems.I am responsible for full lifecycle development of next - generation software, from initial requirement gathering to design, coding, testing, documentation and implementation.
                Known for excellent client-facing skills, I have participated in proposals and presentations that have landed six - figure contracts.I also excel in merging business and user needs into high - quality, cost - effective design solutions while keeping within budgetary constraints.
                My technical expertise includes cross - platform proficiency(Windows, Unix, Linux and VxWorks); fluency in 13 scripting / programming languages(including C, C++, VB, Java, Perl and SQL); and advanced knowledge of developer applications, tools, methodologies and best practices(including OOD, client / server architecture and self - test automation).
                My experience developing user-friendly solutions on time and on budget would enable me to step into a software engineering role at XYZ Company and hit the ground running. I will follow up with you next week, and you may reach me at(215) 555 - 5555.I look forward to speaking with you.",
                CvFileName = "stotinuljeta.pdf",
                CvLetterFileName = "pisamce.pdf",
                GeoLocation = new GeoLocation(45.2396, 19.833549)

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
                CvLetterFileName = "ledubejbee.pdf",
                GeoLocation = new GeoLocation(45.3834, 20.3906)

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
                CvLetterFileName = "ksendzinopismo.pdf",
                GeoLocation = new GeoLocation(44.8125, 20.4612)

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
                CvLetterFileName = "самобогаивишеникога.pdf",
                GeoLocation = new GeoLocation(44.7553, 19.6923)

            });


            return unit;

        }
    }
}
