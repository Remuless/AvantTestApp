using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace testWebApplication.Models
{
    public class Project
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public State status { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
    }
    public interface IProjectRepository
    {
        void Add(Project item);
        IEnumerable<Project> GetAll();
        Project Remove(Guid ID);
        void Update(Project item);
        public Project Find(Guid ID);
    }
    public class ProjectRepository : IProjectRepository
    {
        private static ConcurrentDictionary<Guid, Project> _projects =
              new ConcurrentDictionary<Guid, Project>();

        public ProjectRepository()
        {
            Add(new Project { status = 0 });
        }

        public IEnumerable<Project> GetAll()
        {
            return _projects.Values;
        }

        public void Add(Project item)
        {
            item.ID = Guid.NewGuid();
            _projects[item.ID] = item;
        }

        public Project Remove(Guid ID)
        {
            Project item;
            _projects.TryRemove(ID, out item);
            return item;
        }

        public void Update(Project item)
        {
            _projects[item.ID] = item;
        }
        public Project Find(Guid ID)
        {
            Project item;
            _projects.TryGetValue(ID, out item);
            return item;
        }
    }

    public enum State
    {
        NEW = 0,
        WORK = 1,
        CLOSED = 2,
        DELETED = 3
    }

}
