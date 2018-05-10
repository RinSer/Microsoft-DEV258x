drop database if exists movies;
create database movies;
use movies;

CREATE TABLE `actor` (
  `ActorId` int(11) NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(45) NOT NULL,
  `LastName` varchar(45) NOT NULL,
  PRIMARY KEY (`ActorId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `category` (
  `CategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`CategoryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `film` (
  `FilmId` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `Description` text,
  `ReleaseYear` int(11) DEFAULT NULL,
  `Rating` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`FilmId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `film_actor` (
  `FilmId` int(11) NOT NULL,
  `ActorId` int(11) NOT NULL,
  PRIMARY KEY (`FilmId`,`ActorId`),
  KEY `film_actor_actor_idx` (`ActorId`),
  CONSTRAINT `film_actor_actor` FOREIGN KEY (`ActorId`) REFERENCES `actor` (`ActorId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `film_actor_film` FOREIGN KEY (`FilmId`) REFERENCES `film` (`FilmId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `film_category` (
  `FilmId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  PRIMARY KEY (`FilmId`,`CategoryId`),
  KEY `film_category_category_fk_idx` (`CategoryId`),
  CONSTRAINT `film_category_category_fk` FOREIGN KEY (`CategoryId`) REFERENCES `category` (`CategoryId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `film_category_film_fk` FOREIGN KEY (`FilmId`) REFERENCES `film` (`FilmId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

insert into category (Name) values
	('Action'),
	('Animation'),
	('Children'),
	('Classics'),
	('Comedy'),
	('Documentary'),
	('Drama'),
	('Family'),
	('Foreign'),
	('Games'),
	('Horror'),
	('Music'),
	('New'),
	('Sci-Fi'),
	('Sports'),
	('Travel');

insert into actor (FirstName, LastName) values
	('Mark', 'Hammil'),
	('Harrison', 'Ford'),
	('Carrie', 'Fisher'),
	('Peter', 'Cushing'),
	('Alec', 'Guinness'),
	('Anthony', 'Daniels'),
	('Kenny', 'Baker'),
	('Peter', 'Mayhew'),
	('David', 'Prowse'),
	('Frank', 'Oz'),
	('Phil', 'Brown'),
	('Robert', 'Downy Jr.'),
	('Chris', 'Evans'),
	('Mark', 'Ruffalo'),
	('Chris', 'Hemsworth'),
	('Scarlett', 'Johansson'),
	('Jeremy', 'Renner'),
	('Tom', 'Hiddleston'),
	('Samuel L.', 'Jackson'),
	('Natalie', 'Portman'),
	('Felicity', 'Jones');

insert into film (Title, Description, ReleaseYear, Rating) values
	('Captain America: The Winter Soldier', 'As Steve Rogers struggles to embrace his role in the modern world, he teams up with a fellow Avenger and S.H.I.E.L.D agent, Black Widow, to battle a new threat from history: an assassin known as the Winter Soldier.', 2014, 'PG-13'),
	('The Avengers', 'Earth''s mightiest heroes must come together and learn to fight as a team if they are to stop the mischievous Loki and his alien army from enslaving humanity.', 2012, 'PG-13'),
	('Thor', 'The powerful but arrogant god Thor is cast out of Asgard to live amongst humans in Midgard (Earth), where he soon becomes one of their finest defenders.', 2011, 'PG-13'),
	('Avengers: Age of Ultron', 'When Tony Stark and Bruce Banner try to jump-start a dormant peacekeeping program called Ultron, things go horribly wrong and it''s up to Earth''s mightiest heroes to stop the villainous Ultron from enacting his terrible plan. ', 2015, 'PG-13'),
	('Captain America: The First Avenger', 'Steve Rogers, a rejected military soldier transforms into Captain America after taking a dose of a "Super-Soldier serum". But being Captain America comes at a price as he attempts to take down a war monger and a terrorist organization. ', 2011, 'PG-13'),
	('Star Wars: Episode IV - A New Hope', 'Luke Skywalker joins forces with a Jedi Knight, a cocky pilot, a Wookiee, and two droids to save the galaxy from the Empire''s world-destroying battle-station, while also attempting to rescue Princess Leia from the evil Darth Vader. ', 1977, 'PG'),
	('Star Wars: Episode V - The Empire Strikes Back', 'After the rebels are overpowered by the Empire on their newly established base, Luke Skywalker begins Jedi training with Master Yoda. His friends accept shelter from a questionable ally as Darth Vader hunts them in a plan to capture Luke. ', 1980, 'PG'),
	('Star Wars: Episode VI - Return of the Jedi', 'After a daring mission to rescue Han Solo from Jabba the Hutt, the rebels dispatch to Endor to destroy a more powerful Death Star. Meanwhile, Luke struggles to help Vader back from the dark side without falling into the Emperor''s trap. ', 1983, 'PG'),
	('Star Wars: Episode I - The Phantom Menace', 'Two Jedi Knights escape a hostile blockade to find allies and come across a young boy who may bring balance to the Force, but the long dormant Sith resurface to claim their old glory.', 1999, 'PG'),
	('Star Wars: Episode II - Attack of the Clones', 'Ten years after initially meeting, Anakin Skywalker shares a forbidden romance with Padmé, while Obi-Wan investigates an assassination attempt on the Senator and discovers a secret clone army crafted for the Jedi.', 2002, 'PG'),
	('Star Wars: Episode III - Revenge of the Sith', 'Three years into the Clone Wars, the Jedi rescue Palpatine from Count Dooku. As Obi-Wan pursues a new threat, Anakin acts as a double agent between the Jedi Council and Palpatine and is lured into a sinister plan to rule the galaxy. ', 2005, 'PG-13'),
	('Rogue One', 'Three decades after the Empire''s defeat, a new threat arises in the militant First Order. Stormtrooper defector Finn and spare parts scavenger Rey are caught up in the Resistance''s search for the missing Luke Skywalker.', 2016, 'PG-13');

insert into film_category (FilmId, CategoryId) values
	(1, 1),
	(2, 1),
	(3, 1),
	(4, 1),
	(5, 1),
	(6, 4),
	(7, 4),
	(8, 4),
	(6, 14),
	(7, 14),
	(8, 14),
	(9, 14),
	(10, 14),
	(11, 14),
	(12, 14);

insert into film_actor (FilmId, ActorId) values
	(6, 1),
	(7, 1),
	(8, 1),
	(9, 1),
	(10, 1),
	(11, 1),
	(6, 2),
	(7, 2),
	(8, 2),
	(9, 2),
	(10, 2),
	(11, 2),
	(6, 3),
	(7, 3),
	(8, 3),
	(9, 3),
	(10, 3),
	(11, 3),
	(6, 4),
	(7, 4),
	(8, 4),
	(9, 4),
	(10, 4),
	(11, 4),
	(6, 5),
	(7, 5),
	(8, 5),
	(9, 5),
	(10, 5),
	(11, 5),
	(6, 6),
	(7, 6),
	(8, 6),
	(9, 6),
	(10, 6),
	(11, 6),
	(6, 7),
	(7, 7),
	(8, 7),
	(9, 7),
	(10, 7),
	(11, 7),
	(6, 8),
	(7, 8),
	(8, 8),
	(9, 8),
	(10, 8),
	(11, 8),
	(6, 9),
	(7, 9),
	(8, 9),
	(9, 9),
	(10, 9),
	(11, 9),
	(7, 10),
	(6, 11),
	(7, 11),
	(8, 11),
	(9, 11),
	(10, 11),
	(11, 11),
	(2, 12),
	(4, 12),
	(1, 13),
	(2, 13),
	(4, 13),
	(5, 13),
	(2, 14),
	(4, 14),
	(2, 15),
	(3, 15),
	(4, 15),
	(1, 16),
	(2, 16),
	(4, 16),
	(5, 16),
	(2, 17),
	(4, 17),
	(2, 18),
	(3, 18),
	(1, 19),
	(2, 19),
	(4, 19),
	(5, 19),
	(3, 20),
	(12, 21);


DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `FilmStartsWithTitle`(in prefix varchar(50))
BEGIN

set prefix = concat(prefix, '%');
select * from film where title like prefix order by title;

END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `FindActor`(in find varchar(50))
BEGIN

declare findName varchar(52);
set findName = concat('%', find, '%');
select * from actor where firstname like findName or lastname like findName order by lastname, firstname;

END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `PagedActors`(in pageSize int, in pageNumber int)
BEGIN

declare skip int;
set skip = pageSize * (pageNumber - 1);

select * from actor order by LastName, FirstName limit skip, pageSize;

END$$
DELIMITER ;
