-- Script SQL de criação (conforme enunciado)
CREATE TABLE produto (
  id_produto INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  nome VARCHAR(45) NOT NULL,
  preco DECIMAL(10,2) NOT NULL,
  quantidade INT DEFAULT 0
);
