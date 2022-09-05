namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context
                .Projects
                .Where(p => p.Tasks.Any())
                .ToArray()
            .Select(p => new ExportProjectDto()
            {
                Name = p.Name,
                HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                TasksCount = p.Tasks.Count,
                Tasks = p.Tasks
                    .ToArray()
                    .Select(t => new ExportProjectTaskDto()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
            })
            .OrderByDescending(p => p.TasksCount)
            .ThenBy(p => p.Name)
            .ToArray();

            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                var xmlSerializer = new XmlSerializer(typeof(ExportProjectDto[]), new XmlRootAttribute("Projects"));

                xmlSerializer.Serialize(writer, projects, namespaces);
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var mostBusiestEmployees = context.Employees.ToArray().Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date)).Select(e => new
            {
                Username = e.Username,
                Tasks = e.EmployeesTasks.Where(et => et.Task.OpenDate >= date).OrderByDescending(et => et.Task.DueDate).ThenBy(et => et.Task.Name).Select(et => new
                {
                    TaskName = et.Task.Name,
                    OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                    DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                    LabelType = et.Task.LabelType.ToString(),
                    ExecutionType = et.Task.ExecutionType.ToString()
                }).ToArray()
            }).OrderByDescending(e => e.Tasks.Length).ThenBy(e => e.Username).Take(10).ToArray();

            return JsonConvert.SerializeObject(mostBusiestEmployees, Formatting.Indented);
        }
    }
}

/*ExportProjectDto[] projects = context.Projects.ToArray()).Select(p => new ExportProjectDto()
            {
                Name = p.Name,
                HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                TasksCount = p.Tasks.Count,
                Tasks = p.Tasks.Select(t => new ExportProjectTaskDto()
                {
                    Name = t.Name,
                    Label = t.LabelType.ToString()
                }).OrderBy(t => t.Name).ToArray()
            }).OrderByDescending(p => p.TasksCount).ThenBy(p => p.Name).ToArray();*/