using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestingProject.Models;

namespace TestingProject.Controllers
{
    public class StudentController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "XrEHe8CrorAy8rHHZ8K3wuIDPRbJS2CDoadxXI7V",
            BasePath = "https://studentcrudapp-9daa9-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient? client;

        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Student>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Student>(((JProperty)item).Value.ToString()));
            }

            return View(list);

        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                addStudentToFirebase(student);
                ModelState.AddModelError(String.Empty,"Added Successfully");

            }catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
            }

            return RedirectToAction("Index");
        }

        private void addStudentToFirebase(Student student)
        {
            client=new FireSharp.FirebaseClient(config);
            var data = student;
            PushResponse response = client.Push("Students/", data);
            data.Id = response.Result.name;
            SetResponse setResponse = client.Set("Students/"+data.Id,data);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Students/" + id);
            return RedirectToAction("Index");
        }
    }
}
