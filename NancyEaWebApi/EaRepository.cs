using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EaWebApi.Models;

namespace EaWebApi
{
    public class EaRepository
    {
        private EA.Repository repo = new EA.Repository();
        private EA.Project project;

        public EaRepository(string connection)
        {
            repo.OpenFile(connection);
            project = repo.GetProjectInterface();
        }

        /// <summary>
        /// returns a recursive list of all packages in the project along with all diagrams (that are not embedded)
        /// </summary>
        /// <returns></returns>
        public IList<Package> GetPackages()
        {
            List<Package> packages = new List<Package>();

            foreach (EA.Package p in repo.Models)
            {

                Package child = new Package();
                SerializePackage(child, p);
                packages.Add(child);
            }

            return packages;
        }

        /// <summary>
        /// Returns the information of a diagram
        /// </summary>
        /// <param name="diagramId"></param>
        public Diagram GetDiagram(int diagramId)
        {
            EA.Diagram eadiagram = repo.GetDiagramByID(diagramId);
            Diagram diag = new Diagram();
            SerializeDiagram(diag, eadiagram);
            return diag;
        }

        /// <summary>
        /// Returns the information of a diagram
        /// </summary>
        /// <param name="diagramId"></param>
        public Diagram GetDiagramByGuid(string guid)
        {
            // TODO: validate guid
            EA.Diagram eadiagram = repo.GetDiagramByGuid(guid);
            Diagram diag = new Diagram();
            SerializeDiagram(diag, eadiagram);
            return diag;
        }

        /// <summary>
        /// Exports a diagram to the given path (as png)
        /// </summary>
        /// <param name="diagramId"></param>
        /// <param name="path"></param>
        public void ExportDiagram(int diagramId, string filename)
        {
            repo.OpenDiagram(diagramId);
            project.SaveDiagramImageToFile(filename);
            repo.CloseDiagram(diagramId);
        }

        private void SerializePackage(Package package, EA.Package eapackage)
        {
            package.Name = eapackage.Name;
            package.Id = eapackage.PackageID;
            package.Description = eapackage.Notes;

            foreach (EA.Package p in eapackage.Packages)
            {
                Package child = new Package();
                package.Packages.Add(child);
                SerializePackage(child, p);
            }

            foreach (EA.Diagram ead in eapackage.Diagrams)
            {
                Diagram d = new Diagram();
                package.Diagrams.Add(d);
                SerializeDiagramCompact(d, ead);
            }
        }

        private void SerializeDiagramCompact(Diagram diagram, EA.Diagram eadiagram)
        {
            diagram.Description = eadiagram.Notes;
            diagram.Id = eadiagram.DiagramID;
            diagram.Name = eadiagram.Name;
        }

        private void SerializeDiagram(Diagram diagram, EA.Diagram eadiagram)
        {
            SerializeDiagramCompact(diagram, eadiagram);

            foreach (EA.DiagramObject eadiagramobject in eadiagram.DiagramObjects)
            {
                EA.Element eaelement = repo.GetElementByID(eadiagramobject.ElementID);

                // Notes and Text should not be serialized as they are displayed on the diagram anyhow
                string type = eaelement.Type;
                switch (type)
                {
                    case "Note": continue;
                    case "Text": continue;
                    default: break;
                }

                Element element = new Element();
                diagram.Elements.Add(element);
                SerializeElement(element, eaelement);
            }
        }

        private void SerializeElement(Element element, EA.Element eaelement)
        {
            element.Id = eaelement.ElementID;
            element.Name = eaelement.Name;
            element.Description = eaelement.Notes;
        }



    }
}