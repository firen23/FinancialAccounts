using System;
using System.Collections.Generic;
using System.Linq;
using FinancialAccounts.Models;

namespace Test;

public class TestData
{
    public List<Client> Clients { get; set; }
    public List<Account> Accounts { get; set; }

    public TestData()
    {
        Clients = new List<Client>
        {
            new Client
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Patronymic = "Иванович",
                Birthdate = new DateTime(1990, 6, 5)
            },
            new Client
            {
                FirstName = "Андрей",
                LastName = "Петров",
                Patronymic = "Павлович",
                Birthdate = new DateTime(1991, 5, 6)
            },
            new Client
            {
                FirstName = "Григорий",
                LastName = "Петров",
                Patronymic = "Григорьевич",
                Birthdate = new DateTime(1988, 1, 12)
            },
            new Client
            {
                FirstName = "Михаил",
                LastName = "Петров",
                Patronymic = "Григорьевич",
                Birthdate = new DateTime(1989, 1, 15)
            },
            new Client
            {
                FirstName = "Евгений",
                LastName = "Ложкин",
                Patronymic = "Викторович",
                Birthdate = new DateTime(1965, 11, 5)
            },
            new Client
            {
                FirstName = "Василий",
                LastName = "Ложкин",
                Patronymic = "Викторович",
                Birthdate = new DateTime(1974, 12, 7)
            },
            new Client
            {
                FirstName = "Вячеслав",
                LastName = "Наумов",
                Patronymic = "Иванович",
                Birthdate = new DateTime(2001, 3, 17)
            },
            new Client
            {
                FirstName = "Иван",
                LastName = "Щеткин",
                Patronymic = "Васильевич",
                Birthdate = new DateTime(1991, 3, 15)
            },
            new Client
            {
                FirstName = "Антон",
                LastName = "Милютин",
                Patronymic = "Борисович",
                Birthdate = new DateTime(1988, 9, 8)
            },
            new Client
            {
                FirstName = "Ярослав",
                LastName = "Лампов",
                Patronymic = "Евгеньевич",
                Birthdate = new DateTime(1989, 10, 4)
            },
            new Client
            {
                FirstName = "Максим",
                LastName = "Семиженов",
                Patronymic = "Павлович",
                Birthdate = new DateTime(1990, 1, 29)
            },
            new Client
            {
                FirstName = "Максим",
                LastName = "Ложкин",
                Patronymic = "Игоревич",
                Birthdate = new DateTime(1986, 2, 1)
            },
            new Client
            {
                FirstName = "Филипп",
                LastName = "Ласточкин",
                Patronymic = "Васильевич",
                Birthdate = new DateTime(1979, 5, 14)
            },
            new Client
            {
                FirstName = "Борис",
                LastName = "Галкин",
                Patronymic = "Игоревич",
                Birthdate = new DateTime(1971, 2, 13)
            },
            new Client
            {
                FirstName = "Игорь",
                LastName = "Галкин",
                Patronymic = "Борисович",
                Birthdate = new DateTime(1992, 4, 12)
            },
            new Client
            {
                FirstName = "Данил",
                LastName = "Матросов",
                Patronymic = "Иванович",
                Birthdate = new DateTime(1982, 7, 28)
            },
            new Client
            {
                FirstName = "Кирилл",
                LastName = "Батманов",
                Patronymic = "Викторович",
                Birthdate = new DateTime(1988, 7, 10)
            },
            new Client
            {
                FirstName = "Никита",
                LastName = "Зуев",
                Patronymic = "Никитович",
                Birthdate = new DateTime(1987, 8, 12)
            },
            new Client
            {
                FirstName = "Владимир",
                LastName = "Пухов",
                Patronymic = "Николаевич",
                Birthdate = new DateTime(1996, 3, 2)
            },
            new Client
            {
                FirstName = "Николай",
                LastName = "Змеев",
                Patronymic = "Владимирович",
                Birthdate = new DateTime(1992, 9, 12)
            },
            new Client
            {
                FirstName = "Константин",
                LastName = "Власов",
                Patronymic = "Николаевич",
                Birthdate = new DateTime(1985, 12, 12)
            },
            new Client
            {
                FirstName = "Василий",
                LastName = "Богатырев",
                Patronymic = "Михайлович",
                Birthdate = new DateTime(1986, 1, 13)
            },
            new Client
            {
                FirstName = "Михаил",
                LastName = "Меньшиков",
                Patronymic = "Тарасович",
                Birthdate = new DateTime(1987, 2, 14)
            },
            new Client
            {
                FirstName = "Николай",
                LastName = "Баскин",
                Patronymic = "Юрьевич",
                Birthdate = new DateTime(1988, 3, 15)
            },
            new Client
            {
                FirstName = "Юрий",
                LastName = "Маслов",
                Patronymic = "Глебович",
                Birthdate = new DateTime(1989, 4, 16)
            },////
            new Client
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Patronymic = "Николаевич",
                Birthdate = new DateTime(1958, 6, 5)
            },
            new Client
            {
                FirstName = "Андрей",
                LastName = "Орлов",
                Patronymic = "Павлович",
                Birthdate = new DateTime(1992, 5, 16)
            },
            new Client
            {
                FirstName = "Глеб",
                LastName = "Петров",
                Patronymic = "Григорьевич",
                Birthdate = new DateTime(1988, 11, 12)
            },
            new Client
            {
                FirstName = "Михаил",
                LastName = "Мазаев",
                Patronymic = "Григорьевич",
                Birthdate = new DateTime(1979, 1, 25)
            },
            new Client
            {
                FirstName = "Алексей",
                LastName = "Ложкин",
                Patronymic = "Алексеевич",
                Birthdate = new DateTime(1995, 11, 5)
            },
            new Client
            {
                FirstName = "Игорь",
                LastName = "Конев",
                Patronymic = "Викторович",
                Birthdate = new DateTime(1974, 1, 7)
            },
            new Client
            {
                FirstName = "Вячеслав",
                LastName = "Горшков",
                Patronymic = "Иванович",
                Birthdate = new DateTime(2003, 4, 17)
            },
            new Client
            {
                FirstName = "Ярослав",
                LastName = "Мешков",
                Patronymic = "Васильевич",
                Birthdate = new DateTime(1999, 5, 15)
            },
            new Client
            {
                FirstName = "Алексей",
                LastName = "Водоворотов",
                Patronymic = "Юрьевич",
                Birthdate = new DateTime(1978, 2, 8)
            },
            new Client
            {
                FirstName = "Ярослав",
                LastName = "Любин",
                Patronymic = "Евгеньевич",
                Birthdate = new DateTime(1992, 11, 4)
            },
            new Client
            {
                FirstName = "Максим",
                LastName = "Сараев",
                Patronymic = "Алексеевич",
                Birthdate = new DateTime(1981, 1, 30)
            },
            new Client
            {
                FirstName = "Кирилл",
                LastName = "Батуев",
                Patronymic = "Филиппович",
                Birthdate = new DateTime(1989, 2, 19)
            },
            new Client
            {
                FirstName = "Филипп",
                LastName = "Батуев",
                Patronymic = "Васильевич",
                Birthdate = new DateTime(1962, 5, 14)
            },
            new Client
            {
                FirstName = "Александр",
                LastName = "Галкин",
                Patronymic = "Игоревич",
                Birthdate = new DateTime(1977, 2, 13)
            },
            new Client
            {
                FirstName = "Максим",
                LastName = "Самоваров",
                Patronymic = "Борисович",
                Birthdate = new DateTime(1972, 4, 13)
            },
            new Client
            {
                FirstName = "Данил",
                LastName = "Воеводин",
                Patronymic = "Иванович",
                Birthdate = new DateTime(1988, 7, 22)
            },
            new Client
            {
                FirstName = "Олег",
                LastName = "Батманов",
                Patronymic = "Игоревич",
                Birthdate = new DateTime(1983, 7, 20)
            },
            new Client
            {
                FirstName = "Никита",
                LastName = "Лапенко",
                Patronymic = "Иванович",
                Birthdate = new DateTime(1975, 8, 15)
            },
            new Client
            {
                FirstName = "Владимир",
                LastName = "Прахов",
                Patronymic = "Алексеевич",
                Birthdate = new DateTime(1990, 1, 2)
            },
            new Client
            {
                FirstName = "Николай",
                LastName = "Мышкин",
                Patronymic = "Владимирович",
                Birthdate = new DateTime(1999, 10, 12)
            },
            new Client
            {
                FirstName = "Константин",
                LastName = "Самойлов",
                Patronymic = "Константинович",
                Birthdate = new DateTime(1983, 2, 12)
            },
            new Client
            {
                FirstName = "Тарас",
                LastName = "Богатырев",
                Patronymic = "Михайлович",
                Birthdate = new DateTime(1980, 1, 23)
            },
            new Client
            {
                FirstName = "Леонид",
                LastName = "Меньшиков",
                Patronymic = "Кириллович",
                Birthdate = new DateTime(1989, 12, 14)
            },
            new Client
            {
                FirstName = "Николай",
                LastName = "Лопатов",
                Patronymic = "Леонидович",
                Birthdate = new DateTime(1979, 4, 15)
            },
            new Client
            {
                FirstName = "Юрий",
                LastName = "Гребеньщиков",
                Patronymic = "Александрович",
                Birthdate = new DateTime(1982, 4, 2)
            }
        };
        
        Accounts = Clients.Select(client => new Account {Balance = 0, Client = client}).ToList();
    }
}