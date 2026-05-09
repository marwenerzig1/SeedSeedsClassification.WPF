using SeedsClassification.Core.Classification;
using SeedsClassification.Core.Data;
using SeedsClassification.Core.Evaluation;
using SeedsClassification.Core.Models;
using SeedsClassification.Core.Services;
using SeedsClassification.WPF.Commands;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SeedsClassification.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<GrainBle> TrainData { get; set; } = new();
        public ObservableCollection<GrainBle> TestData { get; set; } = new();

        public ObservableCollection<Experience> Historique { get; set; } = new();

        // 🔥 API USERS
        public ObservableCollection<UserApi> Users { get; set; } = new();

        private UserApi _selectedUser;
        public UserApi SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser)); }
        }

        public ObservableCollection<string> Distances { get; set; }
            = new() { "Euclidienne", "Manhattan" };

        public string SelectedDistance { get; set; } = "Euclidienne";

        private int _k = 3;
        public int K
        {
            get => _k;
            set { _k = value; OnPropertyChanged(nameof(K)); }
        }

        private double _accuracy;
        public string Accuracy => (_accuracy * 100).ToString("0.00") + " %";

        public int[,] ConfusionMatrix { get; set; } = new int[3, 3];

        public int CM00 => ConfusionMatrix[0, 0];
        public int CM01 => ConfusionMatrix[0, 1];
        public int CM02 => ConfusionMatrix[0, 2];
        public int CM10 => ConfusionMatrix[1, 0];
        public int CM11 => ConfusionMatrix[1, 1];
        public int CM12 => ConfusionMatrix[1, 2];
        public int CM20 => ConfusionMatrix[2, 0];
        public int CM21 => ConfusionMatrix[2, 1];
        public int CM22 => ConfusionMatrix[2, 2];

        public RelayCommand LoadCommand { get; }
        public RelayCommand RunCommand { get; }

        // 🔥 API Command
        public RelayCommand LoadUsersCommand { get; }

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(LoadData);
            RunCommand = new RelayCommand(RunKNN);
            LoadUsersCommand = new RelayCommand(async () => await LoadUsers());

            using (var db = new AppDbContext())
                db.Database.EnsureCreated();
        }

        // ========================= CSV =========================

        public void LoadData()
        {
            if (!File.Exists("seeds_dataset_training.csv") ||
                !File.Exists("seeds_dataset_test.csv"))
            {
                MessageBox.Show("Fichiers CSV introuvables !");
                return;
            }

            var loader = new CsvLoader();

            var train = loader.Charger("seeds_dataset_training.csv");
            var test = loader.Charger("seeds_dataset_test.csv");

            TrainData.Clear();
            foreach (var g in train) TrainData.Add(g);

            TestData.Clear();
            foreach (var g in test) TestData.Add(g);
        }

        // ========================= API =========================

        public async Task LoadUsers()
        {
            try
            {
                var api = new ApiService();
                var list = await api.GetUsersAsync();

                Users.Clear();
                foreach (var u in list)
                    Users.Add(u);
            }
            catch
            {
                MessageBox.Show("Erreur lors de l'appel API !");
            }
        }

        // ========================= KNN =========================

        public void RunKNN()
        {
            if (K <= 0 || TrainData.Count == 0)
            {
                MessageBox.Show("Paramètres invalides !");
                return;
            }

            var tree = new KDTree();
            tree.Construire(new List<GrainBle>(TrainData));

            IDistance distance = SelectedDistance == "Manhattan"
                ? new DistanceManhattan()
                : new DistanceEuclidienne();

            var knn = new KNNClassifier(tree, K, distance);

            var evaluator = new EvaluationResult();
            evaluator.Calculer(new List<GrainBle>(TestData), knn);

            _accuracy = evaluator.Accuracy;
            ConfusionMatrix = evaluator.ConfusionMatrix;

            OnPropertyChanged(nameof(Accuracy));
            OnPropertyChanged(nameof(CM00));
            OnPropertyChanged(nameof(CM01));
            OnPropertyChanged(nameof(CM02));
            OnPropertyChanged(nameof(CM10));
            OnPropertyChanged(nameof(CM11));
            OnPropertyChanged(nameof(CM12));
            OnPropertyChanged(nameof(CM20));
            OnPropertyChanged(nameof(CM21));
            OnPropertyChanged(nameof(CM22));

            // 🔥 Auteur API
            string auteur = SelectedUser != null ? SelectedUser.FullName : "Inconnu";

            // 💾 DB
            using (var db = new AppDbContext())
            {
                db.Experiences.Add(new Experience
                {
                    K = K,
                    Distance = SelectedDistance,
                    Accuracy = _accuracy,
                    DateExecution = DateTime.Now,
                    Auteur = auteur
                });

                db.SaveChanges();
            }
        }

        // ========================= HISTORIQUE =========================

        public void ChargerHistorique()
        {
            using (var db = new AppDbContext())
            {
                Historique.Clear();

                var list = db.Experiences
                             .OrderByDescending(x => x.DateExecution)
                             .ToList();

                foreach (var exp in list)
                    Historique.Add(exp);
            }
        }

        // ========================= MVVM =========================

        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}