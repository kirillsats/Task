using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;

namespace TaskMaster
{
    public partial class KN : ContentPage
    {
        // Основные поля игры
        Grid grid2x1, grid3x3;
        Image b; // временная переменная для ячеек игрового поля
        Frame pokazatel;
        Button uus_mang, pravala, temapilti, pole;

        // Кто ходит первым (true – крестики, false – нолики)
        public bool esimene;
        // true – 3x3, false – 4x4
        public bool razmerepole;

        // Результат игры: 1 – победа X, 2 – победа O, -3 – ничья, и т.д.
        int tulemus = 0;

        // Для хранения ходов (1 – крестик, 2 – нолик)
        int[,] Tulemused = new int[4, 4];
        // Для проверки ничьи (значение 4 обозначает, что клетка занята)
        int[,] Nicja = new int[4, 4];

        // Пути к изображениям (поместите их в Resources/Images)
        string[] krest = { "krestik.png", "xred.png" };
        string[] nolik = { "nolik.png", "ored.png" };

        // Размер игрового поля (3 или 4)
        int arazmerpole = 0;

        // Индекс для смены стиля изображений (при желании можно менять)
        int kartinkasmena = 0;

        public KN(int k)
        {
            // Создаем основную сетку с двумя рядами:
            // 1) игровой экран (бОльшая часть),
            // 2) панель с кнопками и индикатором
            grid2x1 = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.DarkGray,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            // Запрос размера поля (3x3 или 4x4)
            Pole_Clicked();

            // Создаем кнопки панели управления
            uus_mang = new Button { Text = "Uus mäng" };
            pravala = new Button { Text = "Reegel" };
            temapilti = new Button { Text = "Välimus" };
            pole = new Button { Text = "Välja suurus" };

            // Индикатор (рамка с изображением) – показывает, чей сейчас ход
            pokazatel = new Frame
            {
                BackgroundColor = Colors.DarkGray,
                WidthRequest = 30,
                HeightRequest = 30,
            };

            // Собираем панель управления в вертикальный стек
            // Сверху – индикатор, ниже – две горизонтальные группы кнопок
            var topIndicator = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children = { pokazatel }
            };

            var buttonsRow1 = new HorizontalStackLayout
            {
                Spacing = 10,
                Children = { uus_mang, pravala }
            };

            var buttonsRow2 = new HorizontalStackLayout
            {
                Spacing = 10,
                Children = { pole, temapilti }
            };

            var controlPanel = new VerticalStackLayout
            {
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children = { topIndicator, buttonsRow1, buttonsRow2 }
            };

            // Добавляем панель управления во вторую строку grid2x1
            grid2x1.Children.Add(controlPanel);
            Grid.SetRow(controlPanel, 1);
            Grid.SetColumn(controlPanel, 0);

            // Подписываем кнопки на события
            uus_mang.Clicked += Uus_mang_Clicked;
            pravala.Clicked += Pravala_Clicked;
            temapilti.Clicked += Temapilti_Clicked;
            pole.Clicked += Pole_Clicked1;

            // Содержимым страницы делаем нашу основную сетку
            Content = grid2x1;
        }

        // Запрос размера поля через кнопку (альтернативный обработчик)
        public async void Pole_Clicked1(object sender, EventArgs e)
        {
            string razmerpoleInput = await DisplayPromptAsync(
                "Välja suurus",
                "Tee valiku 3x3 - 1 või 4x4 - 2",
                initialValue: "1",
                maxLength: 1,
                keyboard: Keyboard.Numeric);

            if (razmerpoleInput == "1")
                razmerepole = true;
            else if (razmerpoleInput == "2")
                razmerepole = false;
            else
                razmerepole = true;
        }

        // Запрос размера поля при запуске игры (вызывается из конструктора)
        public async void Pole_Clicked()
        {
            string razmerpoleInput = await DisplayPromptAsync(
                "Välja suurus",
                "Tee valiku 3x3 - 1 või 4x4 - 2",
                initialValue: "1",
                maxLength: 1,
                keyboard: Keyboard.Numeric);

            if (razmerpoleInput == "1")
            {
                razmerepole = true;
                Uus_mang();
            }
            else if (razmerpoleInput == "2")
            {
                razmerepole = false;
                Uus_mang();
            }
            else
            {
                // Если пользователь ввел что-то не то – повторяем запрос
                Pole_Clicked();
            }
        }

        // Запуск новой игры
        private void Uus_mang_Clicked(object sender, EventArgs e)
        {
            // Асинхронно показываем диалог, но не ждем его ответа здесь.
            _ = DisplayAlert("Uus mäng", "Kas tõesti tahad uus mäng?", "Tahan küll!", "Ei taha!")
                   .ContinueWith(task =>
                   {
                       if (task.Result)
                       {
                           // Логика, если пользователь выбрал "Tahan küll!"
                           Uus_mang();
                       }
                       else
                       {
                           // Логика, если пользователь выбрал "Ei taha!"
                       }
                   });
        }

        // Запуск новой игры (выполняется, если пользователь подтвердил)
        public void Uus_mang()
        {
            // Определяем, кто ходит первым
            Kes_on_Esimene();
            tulemus = -2;

            // Инициализируем массивы в зависимости от выбранного размера поля
            if (razmerepole == true)
            {
                Tulemused = new int[3, 3];
                Nicja = new int[3, 3];
                arazmerpole = 3;
            }
            else
            {
                Tulemused = new int[4, 4];
                Nicja = new int[4, 4];
                arazmerpole = 4;
            }

            // Создаем новое игровое поле
            grid3x3 = new Grid
            {
                BackgroundColor = Colors.White
            };

            // Добавляем ряды и колонки
            for (int i = 0; i < arazmerpole; i++)
            {
                grid3x3.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                grid3x3.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Заполняем игровое поле «фоновыми» изображениями
            // и добавляем распознавание касаний (TapGestureRecognizer)
            for (int row = 0; row < arazmerpole; row++)
            {
                for (int col = 0; col < arazmerpole; col++)
                {
                    b = new Image { Source = "fon.jpg" };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += Tap_Tapped;
                    b.GestureRecognizers.Add(tapGestureRecognizer);

                    grid3x3.Children.Add(b);
                    Grid.SetRow(b, row);
                    Grid.SetColumn(b, col);
                }
            }

            // Добавляем поле на страницу
            grid2x1.Children.Add(grid3x3);
            Grid.SetRow(grid3x3, 0);
            Grid.SetColumn(grid3x3, 0);
        }

        // Обработчик тапов на клетках
        private void Tap_Tapped(object sender, EventArgs e)
        {
            var tappedImage = sender as Image;
            var row = Grid.GetRow(tappedImage);
            var col = Grid.GetColumn(tappedImage);

            // Игрок может поставить X или O только в пустую клетку
            if (Nicja[row, col] == 0) // если клетка не занята
            {
                if (esimene)
                {
                    tappedImage.Source = krest[kartinkasmena];
                    Tulemused[row, col] = 1;  // X
                    Nicja[row, col] = 4;  // Клетка занята
                    esimene = false;
                    pokazatel.Content = new Image { Source = nolik[0] };
                }
                else
                {
                    tappedImage.Source = nolik[kartinkasmena];
                    Tulemused[row, col] = 2;  // O
                    Nicja[row, col] = 4;  // Клетка занята
                    esimene = true;
                    pokazatel.Content = new Image { Source = krest[0] };
                }

                // После хода игрока проверяем на победу
                CheckWinner();
            }
        }

        private void CheckWinner()
        {
            // Проверка всех линий на победу
            for (int i = 0; i < arazmerpole; i++)
            {
                // Горизонтали
                if (Tulemused[i, 0] == Tulemused[i, 1] && Tulemused[i, 1] == Tulemused[i, 2] && Tulemused[i, 0] != 0)
                {
                    DisplayWinner(Tulemused[i, 0]);
                    return;
                }

                // Вертикали
                if (Tulemused[0, i] == Tulemused[1, i] && Tulemused[1, i] == Tulemused[2, i] && Tulemused[0, i] != 0)
                {
                    DisplayWinner(Tulemused[0, i]);
                    return;
                }
            }

            // Проверка диагоналей (для 3x3)
            if (arazmerpole == 3)
            {
                // Первая диагональ
                if (Tulemused[0, 0] == Tulemused[1, 1] && Tulemused[1, 1] == Tulemused[2, 2] && Tulemused[0, 0] != 0)
                {
                    DisplayWinner(Tulemused[0, 0]);
                    return;
                }

                // Вторая диагональ
                if (Tulemused[0, 2] == Tulemused[1, 1] && Tulemused[1, 1] == Tulemused[2, 0] && Tulemused[0, 2] != 0)
                {
                    DisplayWinner(Tulemused[0, 2]);
                    return;
                }
            }

            // Проверка ничьей
            bool isDraw = true;
            for (int row = 0; row < arazmerpole; row++)
            {
                for (int col = 0; col < arazmerpole; col++)
                {
                    if (Nicja[row, col] == 0)
                    {
                        isDraw = false;
                        break;
                    }
                }
            }

            if (isDraw)
            {
                DisplayAlert("Draw", "The game ended in a draw!", "OK");
            }
        }

        private void DisplayWinner(int winner)
        {
            // Выводим сообщение о победителе
            string winnerMessage = winner == 1 ? "Player X wins!" : "Player O wins!";
            DisplayAlert("Game Over", winnerMessage, "OK");
            tulemus = winner; // Обновляем результат
        }

        private async void Pravala_Clicked(object sender, EventArgs e)
        {
            // Тут можно показать описание правил игры
            await DisplayAlert("Reegel", "Игроки по очереди ставят X или O в клетку, цель - собрать 3 одинаковых символа подряд по вертикали, горизонтали или диагонали.", "OK");
        }

        private async void Temapilti_Clicked(object sender, EventArgs e)
        {
            // Тут можно сменить стиль или тему игры
            await DisplayAlert("Välimus", "Выберите стиль изображения или тему.", "OK");
        }

        // Запрос, кто будет ходить первым
        public async void Kes_on_Esimene()
        {
            string esimineInput = await DisplayPromptAsync(
                "Kes on esimene?",
                "Tee valiku X - 1 või O - 2",
                initialValue: "1",
                maxLength: 1,
                keyboard: Keyboard.Numeric);

            if (esimineInput == "1")
            {
                esimene = true; // Первый ходит X
            }
            else if (esimineInput == "2")
            {
                esimene = false; // Первый ходит O
            }
            else
            {
                Kes_on_Esimene(); // Запрашиваем снова
            }
        }
    }
}
