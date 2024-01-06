CREATE TABLE IF NOT EXISTS users (
	id					SERIAL PRIMARY KEY,
	name				VARCHAR (100) NOT NULL,
	name_lower_case		VARCHAR (100) UNIQUE NOT NULL,
	password_hash		VARCHAR (255) NOT NULL,
	roles				VARCHAR (50) []
);

CREATE TABLE IF NOT EXISTS authors (
	id					SERIAL PRIMARY KEY,
	code				VARCHAR (100) UNIQUE NOT NULL,
	born				VARCHAR (20),
	died				VARCHAR (20)
);

CREATE TABLE IF NOT EXISTS authors_infos (
	author_code			VARCHAR (100) NOT NULL,
	language			VARCHAR (10) NOT NULL,
	full_name			VARCHAR (100),
	description			TEXT,
	PRIMARY KEY (author_code, language),
	CONSTRAINT fk_author_code FOREIGN KEY (author_code) REFERENCES authors (code)
);
CREATE INDEX IF NOT EXISTS author_code_idx ON authors_infos (author_code);

CREATE TABLE IF NOT EXISTS files (
	id					SERIAL PRIMARY KEY,
	code				TEXT UNIQUE NOT NULL,
	format				TEXT,
	size				INTEGER
);

CREATE TABLE IF NOT EXISTS files_links (
	file_code			TEXT NOT NULL,
	type				TEXT NOT NULL,
	path				TEXT NOT NULL,
	PRIMARY KEY (file_code, type, path),
	CONSTRAINT fk_file_code FOREIGN KEY (file_code) REFERENCES files (code)
);
CREATE INDEX IF NOT EXISTS file_code_idx ON files_links (file_code);

CREATE TABLE IF NOT EXISTS works (
	id					SERIAL PRIMARY KEY,
	code				VARCHAR (100) UNIQUE NOT NULL,
	publish				VARCHAR (20),
	language			VARCHAR (10),
	original_code		VARCHAR (100),
	CONSTRAINT fk_original_code FOREIGN KEY (original_code) REFERENCES works (code)
);

CREATE TABLE IF NOT EXISTS works_infos (
	work_code			VARCHAR (100) NOT NULL,
	language			VARCHAR (10) NOT NULL,
	name				VARCHAR (100),
	description			TEXT,
	PRIMARY KEY (work_code, language),
	CONSTRAINT fk_work_code FOREIGN KEY (work_code) REFERENCES works (code)
);
CREATE INDEX IF NOT EXISTS work_code_idx ON works_infos (work_code);

CREATE TABLE IF NOT EXISTS works_authors (
	work_code			VARCHAR (100) NOT NULL,
	author_code			VARCHAR (100) NOT NULL,
	PRIMARY KEY (work_code, author_code),
	CONSTRAINT fk_work_code FOREIGN KEY (work_code) REFERENCES works (code),
	CONSTRAINT fk_author_code FOREIGN KEY (author_code) REFERENCES authors (code)
);
CREATE INDEX IF NOT EXISTS work_code_idx ON works_authors (work_code);

CREATE TABLE IF NOT EXISTS works_sub_works (
	work_code				VARCHAR (100) NOT NULL,
	sub_work_code			VARCHAR (100) NOT NULL,
	PRIMARY KEY (work_code, sub_work_code),
	CONSTRAINT fk_work_code FOREIGN KEY (work_code) REFERENCES works (code),
	CONSTRAINT fk_sub_work_code FOREIGN KEY (sub_work_code) REFERENCES works (code)
);
CREATE INDEX IF NOT EXISTS work_code_idx ON works_sub_works (work_code);

CREATE TABLE IF NOT EXISTS works_files (
	work_code				VARCHAR (100) NOT NULL,
	file_code				VARCHAR (100) NOT NULL,
	PRIMARY KEY (work_code, file_code),
	CONSTRAINT fk_work_code FOREIGN KEY (work_code) REFERENCES works (code),
	CONSTRAINT fk_file_code FOREIGN KEY (file_code) REFERENCES files (code)
);
CREATE INDEX IF NOT EXISTS work_code_idx ON works_files (work_code);