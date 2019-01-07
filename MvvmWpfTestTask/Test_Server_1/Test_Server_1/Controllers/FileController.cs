using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Server_1.Controllers
{
    using Newtonsoft.Json;
    using Test_Server_1.Models;

    public class FileController
    {
        private static readonly string _jsonName = "notes.json";
        private static readonly string _folderPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string filePath = Path.Combine(_folderPath, _jsonName);

        public List<NoteModel> NoteList { get; private set; }

        private static FileController _instance;
        private static readonly object syncRoot = new Object();

        private FileController() { }

        public static FileController GetInstance()
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new FileController();
                }
            }
            return _instance;
        }

        public void Initialize()
        {
            if (!File.Exists(filePath))
                File.Create(filePath);

            LoadJson();
        }

        private async Task<bool> SaveJson()
        {
            string json = await SerializeModelsAsync(NoteList);
            await File.WriteAllTextAsync(filePath, json, encoding: Encoding.UTF8);

            if(File.Exists(filePath))
                return await Task.FromResult(true);
            
            return await Task.FromResult(false);
        }
        private async void LoadJson()
        {
            NoteList = new List<NoteModel>();
            var result = await File.ReadAllTextAsync(filePath, encoding: Encoding.UTF8);

            if (!result.Equals(string.Empty))
                NoteList = await DeserializeModelsAsync(result);
        }

        public async Task<bool> AddNote(string json)
        {
            NoteModel newNote = await DeserializeModelAsync(json);

            if (!newNote.NoteID.Equals(string.Empty) &&
                !newNote.Headline.Equals(string.Empty) &&
                !newNote.Description.Equals(string.Empty) &&
                !newNote.Date.Equals(string.Empty) &&
                !newNote.Image.Equals(string.Empty))
            {
                NoteList.Add(newNote);
            }

            return await Task.FromResult(await SaveJson());
        }
        public async Task<bool> EditNote(string json)
        {
            NoteModel newNote = await DeserializeModelAsync(json);

            var searchNote = NoteList.Where(x => x.NoteID == newNote.NoteID);
            foreach (var note in searchNote)
            {
                note.Headline = newNote.Headline;
                note.Description = newNote.Description;
                note.Image = newNote.Image;
            }

            return await Task.FromResult(await SaveJson());
        }
        public async Task<bool> DeleteNote(string json, bool isMultiple = false)
        {
            if (isMultiple)
            {
                List<NoteModel> noteList = await DeserializeModelsAsync(json);
                foreach (var note in noteList)
                {
                    var model = NoteList.AsParallel().Where(x => x.NoteID == note.NoteID).FirstOrDefault();
                    NoteList.Remove(model);
                }
            }
            else
            {
                NoteModel note = await DeserializeModelAsync(json);
                var model = NoteList.Where( x => x.NoteID == note.NoteID).FirstOrDefault();
                NoteList.Remove(model);
            }

            return await Task.FromResult(await SaveJson());
        }

        public async Task<string> GetNotes()
        {
            return await Task.FromResult(await SerializeModelsAsync(NoteList));
        }

        private async Task<NoteModel> DeserializeModelAsync(string json)
        {
            return await Task.FromResult(JsonConvert.DeserializeObject<NoteModel>(json));
        }
        private async Task<List<NoteModel>> DeserializeModelsAsync(string json)
        {
            return await Task.FromResult(JsonConvert.DeserializeObject<List<NoteModel>>(json));
        }

        private async Task<string> SerializeModelsAsync(List<NoteModel> listModels)
        {
            return await Task.FromResult(JsonConvert.SerializeObject(listModels));
        }

    }
}
