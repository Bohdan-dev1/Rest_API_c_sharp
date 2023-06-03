SELECT 
dbo.Reader_Person.Person_ID, dbo.Issued_Books.ID_Order, dbo.Reader_Person.Full_Name, dbo.Reader_Person.Numer_Phone, dbo.Reader_Person.Email, 
dbo.Reader_Person.Address, dbo.Reader_Person.NIE_Code, dbo.Reader_Person.Add_Details_Person
FROM dbo.Reader_Person
JOIN dbo.Issued_Books 
ON dbo.Issued_Books.ID_Reader = dbo.Reader_Person.Person_ID
JOIN dbo.Book
ON dbo.Book.ID_Book = dbo.Issued_Books.ID_book
WHERE dbo.Reader_Person.Full_Name Like '%Some%';

SELECT 
dbo.Issued_Books.ID_Order, dbo.Book.Name_Book, dbo.Issued_Books.Date_Get, dbo.Issued_Books.Date_Return,  dbo.Issued_Books.Date_Need_Return
FROM dbo.Reader_Person
JOIN dbo.Issued_Books 
ON dbo.Issued_Books.ID_Reader = dbo.Reader_Person.Person_ID
JOIN dbo.Book
ON dbo.Book.ID_Book = dbo.Issued_Books.ID_book
;

	