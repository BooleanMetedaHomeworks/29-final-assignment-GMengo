        CREATE TABLE Categoria
    (
        [Id] INT NOT NULL identity(1,1) PRIMARY KEY,
        [nome] VARCHAR(100) NOT NULL
    )

        CREATE TABLE Piatto
    (
        [Id] INT NOT NULL identity(1,1) PRIMARY KEY,
        [nome] VARCHAR(50) NOT NULL,
        [descrizione] VARCHAR(255) NOT NULL,
        [prezzo] FLOAT NOT NULL,
        [categoriaId] INT, 
        foreign key (categoriaId) REFERENCES Categoria(Id) on delete set null
    )

        CREATE TABLE Menu
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [nome] VARCHAR(50) not null
    )

        CREATE TABLE PiattoMenu
    (
        piattoId INT,
        menuId INT,
        PRIMARY KEY (piattoId, menuId),
        FOREIGN KEY (piattoId) REFERENCES Piatto(Id) on delete cascade,
        FOREIGN KEY (menuId) REFERENCES Menu(Id) on delete cascade
    )

    CREATE TABLE Utente
    (
        Id int primary key identity (1,1) not null,
        Email nvarchar(50) not null unique,
        PasswordHash nvarchar(255) not null
    );


    CREATE TABLE Ruolo
    (
        Id INT PRIMARY KEY IDENTITY (1, 1) NOT NULL,
        Nome NVARCHAR(50) NOT NULL UNIQUE
    );

    CREATE TABLE UtenteRuolo
    (
        UtenteId INT NOT NULL,
        RuoloId INT NOT NULL,
        PRIMARY KEY (UtenteId, RuoloId),
        FOREIGN KEY (UtenteId) REFERENCES Utente(Id),
        FOREIGN KEY (RuoloId) REFERENCES Ruolo(Id)
    );

    insert into utente(email,passwordhash) values ('admin@prova.com','AQAAAAIAAYagAAAAEEOoenBKf+Hd6FfY57xO9/Ik08TsH5Vi7H7+cbhDkyqyyoiWpx6sLnFC8WLiJ3ys6g=='); -- per testare il login la password è l' hash di "prova"
    insert into ruolo (nome) values ('admin');
    insert into utenteruolo(utenteid,ruoloid) values (1,1); -- diamo il ruolo di admin al primo utente inserito nel DB in questo caso all' utente con l' email: admin@prova.com 