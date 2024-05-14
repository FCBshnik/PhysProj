CREATE TABLE IF NOT EXISTS works_sub_works_authors (
	work_code			VARCHAR (100) NOT NULL,
	author_code			VARCHAR (100) NOT NULL,
	PRIMARY KEY (work_code, author_code),
	CONSTRAINT fk_work_code FOREIGN KEY (work_code) REFERENCES works (code),
	CONSTRAINT fk_author_code FOREIGN KEY (author_code) REFERENCES authors (code)
);
CREATE INDEX IF NOT EXISTS work_code_idx ON works_sub_works_authors (work_code);