using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

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

        // Смена темы (изменение случайных цветов)
        private void Temapilti_Clicked(object sender, EventArgs e)
        {
            Random r = new Random();
            // Меняем цвет индикатора и фона панели
            pokazatel.BackgroundColor = Color.FromRgb(r.Next(256), r.Next(256), r.Next(256));
            grid2x1.BackgroundColor = Color.FromRgb(r.Next(256), r.Next(256), r.Next(256));
        }

        // Отображение правил игры
        private void Pravala_Clicked(object sender, EventArgs e)
        {
            DisplayAlert(
                "Reegel",
                "3x3 - \n" +
                "Mängijad kordamööda panevad vabad lahtrid valdkonnas 3x3 märgid (üks on alati ristid, teine on alati null). " +
                "Võidab esimene, kes rivistab 3 oma tükki vertikaalselt, horisontaalselt või diagonaalselt. Esimese käigu teeb " +
                "mängija, kes paneb risti.\n\n" +
                "4x4 - \n" +
                "Tänu suuruse suurenemisele ilmub palju uusi käike ja siis muutub duell pingelisemaks. Reeglid jäävad samaks – " +
                "üks mängija saab joonistada ainult riste ja teine ringe. On vaja teha rida neljast identsest märgist " +
                "horisontaalselt, diagonaalselt või vertikaalselt.",
                "Ok");
        }

        // Запрос, кто будет ходить первым, и установка индикатора
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
                esimene = true;
                pokazatel.Content = new Image { Source = krest[0] };
            }
            else if (esimineInput == "2")
            {
                esimene = false;
                pokazatel.Content = new Image { Source = nolik[0] };
            }
            else
            {
                // Если введено что-то не то, повторяем запрос
                Kes_on_Esimene();
            }
        }

        // Обработчик кнопки "Uus mäng" – запуск новой игры
        private void Uus_mang_Clicked(object sender, EventArgs e)
        {
            Uus_mang();
            temapilti.IsEnabled = true;
            if (grid3x3 != null)
                grid3x3.IsEnabled = true;
        }

        // Запуск новой игры
        public async void Uus_mang()
        {
            bool uus = await DisplayAlert(
                "Uus mäng",
                "Kas tõesti tahad uus mäng?",
                "Tahan küll!",
                "Ei taha!");

            if (uus)
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
                        grid3x3.Children.Add(b);
                        Grid.SetRow(b, row);
                        Grid.SetColumn(b, col);

                        var tap = new TapGestureRecognizer();
                        tap.Tapped += Tap_Tapped;
                        b.GestureRecognizers.Add(tap);
                    }
                }

                // Добавляем поле в верхнюю часть экрана (Row = 0)
                grid2x1.Children.Add(grid3x3);
                Grid.SetRow(grid3x3, 0);
                Grid.SetColumn(grid3x3, 0);
            }
        }

        // Проверка победы для поля 3x3
        public int Kontroll()
        {
            // Проверка X (1)
            if (
                (Tulemused[0, 0] == 1 && Tulemused[1, 0] == 1 && Tulemused[2, 0] == 1) ||
                (Tulemused[0, 1] == 1 && Tulemused[1, 1] == 1 && Tulemused[2, 1] == 1) ||
                (Tulemused[0, 2] == 1 && Tulemused[1, 2] == 1 && Tulemused[2, 2] == 1) ||
                (Tulemused[0, 0] == 1 && Tulemused[0, 1] == 1 && Tulemused[0, 2] == 1) ||
                (Tulemused[1, 0] == 1 && Tulemused[1, 1] == 1 && Tulemused[1, 2] == 1) ||
                (Tulemused[2, 0] == 1 && Tulemused[2, 1] == 1 && Tulemused[2, 2] == 1) ||
                (Tulemused[0, 0] == 1 && Tulemused[1, 1] == 1 && Tulemused[2, 2] == 1) ||
                (Tulemused[0, 2] == 1 && Tulemused[1, 1] == 1 && Tulemused[2, 0] == 1)
               )
            {
                tulemus = 1;
            }
            // Проверка O (2)
            else if (
                (Tulemused[0, 0] == 2 && Tulemused[1, 0] == 2 && Tulemused[2, 0] == 2) ||
                (Tulemused[0, 1] == 2 && Tulemused[1, 1] == 2 && Tulemused[2, 1] == 2) ||
                (Tulemused[0, 2] == 2 && Tulemused[1, 2] == 2 && Tulemused[2, 2] == 2) ||
                (Tulemused[0, 0] == 2 && Tulemused[0, 1] == 2 && Tulemused[0, 2] == 2) ||
                (Tulemused[1, 0] == 2 && Tulemused[1, 1] == 2 && Tulemused[1, 2] == 2) ||
                (Tulemused[2, 0] == 2 && Tulemused[2, 1] == 2 && Tulemused[2, 2] == 2) ||
                (Tulemused[0, 0] == 2 && Tulemused[1, 1] == 2 && Tulemused[2, 2] == 2) ||
                (Tulemused[0, 2] == 2 && Tulemused[1, 1] == 2 && Tulemused[2, 0] == 2)
               )
            {
                tulemus = 2;
            }
            // Проверка на ничью (все клетки заняты – значение 4)
            else if (
                Tulemused[0, 0] == 4 && Tulemused[1, 0] == 4 && Tulemused[2, 0] == 4 &&
                Tulemused[0, 1] == 4 && Tulemused[1, 1] == 4 && Tulemused[2, 1] == 4 &&
                Tulemused[0, 2] == 4 && Tulemused[1, 2] == 4 && Tulemused[2, 2] == 4
            )
            {
                tulemus = -3;
            }
            return tulemus;
        }

        // Проверка победы для поля 4x4
        public int Kontroll4na4()
        {
            // Победа X (1)
            if (
                (Tulemused[0, 0] == 1 && Tulemused[1, 0] == 1 && Tulemused[2, 0] == 1 && Tulemused[3, 0] == 1) ||
                (Tulemused[0, 1] == 1 && Tulemused[1, 1] == 1 && Tulemused[2, 1] == 1 && Tulemused[3, 1] == 1) ||
                (Tulemused[0, 2] == 1 && Tulemused[1, 2] == 1 && Tulemused[2, 2] == 1 && Tulemused[3, 2] == 1) ||
                (Tulemused[0, 3] == 1 && Tulemused[1, 3] == 1 && Tulemused[2, 3] == 1 && Tulemused[3, 3] == 1) ||
                (Tulemused[0, 0] == 1 && Tulemused[0, 1] == 1 && Tulemused[0, 2] == 1 && Tulemused[0, 3] == 1) ||
                (Tulemused[1, 0] == 1 && Tulemused[1, 1] == 1 && Tulemused[1, 2] == 1 && Tulemused[1, 3] == 1) ||
                (Tulemused[2, 0] == 1 && Tulemused[2, 1] == 1 && Tulemused[2, 2] == 1 && Tulemused[2, 3] == 1) ||
                (Tulemused[3, 0] == 1 && Tulemused[3, 1] == 1 && Tulemused[3, 2] == 1 && Tulemused[3, 3] == 1) ||
                (Tulemused[0, 0] == 1 && Tulemused[1, 1] == 1 && Tulemused[2, 2] == 1 && Tulemused[3, 3] == 1) ||
                (Tulemused[0, 3] == 1 && Tulemused[1, 2] == 1 && Tulemused[2, 1] == 1 && Tulemused[3, 0] == 1)
            )
            {
                tulemus = 1;
            }
            // Победа O (2)
            else if (
                (Tulemused[0, 0] == 2 && Tulemused[1, 0] == 2 && Tulemused[2, 0] == 2 && Tulemused[3, 0] == 2) ||
                (Tulemused[0, 1] == 2 && Tulemused[1, 1] == 2 && Tulemused[2, 1] == 2 && Tulemused[3, 1] == 2) ||
                (Tulemused[0, 2] == 2 && Tulemused[1, 2] == 2 && Tulemused[2, 2] == 2 && Tulemused[3, 2] == 2) ||
                (Tulemused[0, 3] == 2 && Tulemused[1, 3] == 2 && Tulemused[2, 3] == 2 && Tulemused[3, 3] == 2) ||
                (Tulemused[0, 0] == 2 && Tulemused[0, 1] == 2 && Tulemused[0, 2] == 2 && Tulemused[0, 3] == 2) ||
                (Tulemused[1, 0] == 2 && Tulemused[1, 1] == 2 && Tulemused[1, 2] == 2 && Tulemused[1, 3] == 2) ||
                (Tulemused[2, 0] == 2 && Tulemused[2, 1] == 2 && Tulemused[2, 2] == 2 && Tulemused[2, 3] == 2) ||
                (Tulemused[3, 0] == 2 && Tulemused[3, 1] == 2 && Tulemused[3, 2] == 2 && Tulemused[3, 3] == 2) ||
                (Tulemused[0, 0] == 2 && Tulemused[1, 1] == 2 && Tulemused[2, 2] == 2 && Tulemused[3, 3] == 2) ||
                (Tulemused[0, 3] == 2 && Tulemused[1, 2] == 2 && Tulemused[2, 1] == 2 && Tulemused[3, 0] == 2)
            )
            {
                tulemus = 2;
            }
            // Ничья (все клетки заняты)
            else if (
                Tulemused[0, 0] == 4 && Tulemused[1, 0] == 4 && Tulemused[2, 0] == 4 && Tulemused[3, 0] == 4 &&
                Tulemused[0, 1] == 4 && Tulemused[1, 1] == 4 && Tulemused[2, 1] == 4 && Tulemused[3, 1] == 4 &&
                Tulemused[0, 2] == 4 && Tulemused[1, 2] == 4 && Tulemused[2, 2] == 4 && Tulemused[3, 2] == 4 &&
                Tulemused[0, 3] == 4 && Tulemused[1, 3] == 4 && Tulemused[2, 3] == 4 && Tulemused[3, 3] == 4
            )
            {
                tulemus = -3;
            }
            return tulemus;
        }

        // Проверяем результат и выводим сообщение
        public void Lopp()
        {
            // Вызываем соответствующую проверку
            tulemus = razmerepole ? Kontroll() : Kontroll4na4();

            if (tulemus == 1)
            {
                DisplayAlert("Võit", "X on võitja!", "Ok");
                grid3x3.IsEnabled = false;
                pole.IsEnabled = true;
            }
            else if (tulemus == 2)
            {
                DisplayAlert("Võit", "O on võitja!", "Ok");
                grid3x3.IsEnabled = false;
                pole.IsEnabled = true;
            }
            else if (tulemus == -3)
            {
                DisplayAlert("Võit", "Viik!", "Ok");
            }
        }

        // Обработчик касания по ячейке игрового поля
        public void Tap_Tapped(object sender, EventArgs e)
        {
            var cellImage = (Image)sender;
            int row = Grid.GetRow(cellImage);
            int col = Grid.GetColumn(cellImage);

            // Если ходят крестики
            if (esimene)
            {
                cellImage = new Image { Source = krest[kartinkasmena] };
                pokazatel.Content = new Image { Source = nolik[0] };
                esimene = false;
                Tulemused[row, col] = 1;
                Nicja[row, col] = 4;
            }
            else
            {
                // Ход ноликов
                cellImage = new Image { Source = nolik[kartinkasmena] };
                pokazatel.Content = new Image { Source = krest[0] };
                esimene = true;
                Tulemused[row, col] = 2;
                Nicja[row, col] = 4;
            }

            // Чтобы заменить фон-картинку на выбранную (крестик или нолик),
            // заново добавляем Image в ячейку
            grid3x3.Children.Add(cellImage);
            Grid.SetRow(cellImage, row);
            Grid.SetColumn(cellImage, col);

            // Блокируем смену темы и размера поля, пока игра не закончится
            temapilti.IsEnabled = false;
            pole.IsEnabled = false;

            // Проверяем результат
            Lopp();
        }
    }
}