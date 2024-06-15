using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TheTriviaProject.View
{
    public partial class MainPage : Page
    {
        private SQLiteConnection connection;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;
        private int currentQuestionIndex = 0;
        private int totalQuestions = 5;
        private HashSet<string> askedQuestions = new HashSet<string>();
        private int score = 0;

        public MainPage()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadQuestions();
        }

        private void InitializeDatabaseConnection()
        {
            string connectionString = "Data Source=C:\\Users\\edenk\\Desktop\\TheTriviaProjec-master\\TheTriviaProject\\MyData.db;Version=3;";
            connection = new SQLiteConnection(connectionString);
            dataAdapter = new SQLiteDataAdapter("SELECT * FROM Questions", connection);
            dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
        }

        private void LoadQuestions()
        {
            if (currentQuestionIndex < totalQuestions)
            {
                if (dataTable.Rows.Count > 0)
                {
                    // Shuffle the rows of the DataTable
                    ShuffleDataTableRows(dataTable);

                    DataRow row = dataTable.Rows[currentQuestionIndex];
                    questionTextBlock.Text = row["QuestionText"].ToString();
                    string[] answers = row["Answers"].ToString().Split(',');
                    string correctAnswer = row["Answer"].ToString();

                    List<string> options = new List<string>(answers) { correctAnswer };

                    // Shuffle the options array
                    RandomizeOptions(options);

                    // Assign shuffled options to answer buttons
                    ans1.Content = options[0];
                    ans2.Content = options[1];
                    ans3.Content = options[2];
                    ans4.Content = options[3];

                    currentQuestionIndex++;
                }
                else
                {
                    MessageBox.Show("No questions available");
                    // Handle the case when no questions are available in the DataTable
                }
            }
            else
            {
                MessageBox.Show("Trivia Ended");
            }
        }

        private void ShuffleDataTableRows(DataTable dataTable)
        {
            Random rnd = new Random();
            int n = dataTable.Rows.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                DataRow tempRow = dataTable.NewRow();
                tempRow.ItemArray = dataTable.Rows[k].ItemArray;
                dataTable.Rows[k].ItemArray = dataTable.Rows[n].ItemArray;
                dataTable.Rows[n].ItemArray = tempRow.ItemArray;
            }
        }

        private void RandomizeOptions(List<string> options)
        {
            Random rnd = new Random();
            for (int i = options.Count - 1; i > 0; i--)
            {
                int index = rnd.Next(i + 1);
                string temp = options[i];
                options[i] = options[index];
                options[index] = temp;
            }
        }

        private void CheckAnswerAndLoadNext(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string selectedAnswer = clickedButton.Content.ToString();
            DataRow currentQuestionRow = dataTable.Rows[currentQuestionIndex - 1];
            string correctAnswer = currentQuestionRow["Answer"].ToString();

            if (selectedAnswer == correctAnswer)
            {
                score += 20;
                MessageBox.Show("Correct", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Incorrect", "Result", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            scoreTextBlock.Text = $"Score: {score}";
            LoadQuestions();
        }
    }
}
