# Controle de Gastos Residenciais – Backend (.NET)

Este projeto representa o **backend** de um sistema de controle de gastos residenciais.

A aplicação foi construída utilizando **ASP.NET Core Web API**, com foco em clareza de regras de negócio, organização do código e boas práticas de desenvolvimento em .NET.

---

## 🎯 Objetivo

Implementar um sistema capaz de gerenciar **pessoas, categorias e transações financeiras**, aplicando regras de negócio específicas e disponibilizando **relatórios consolidados** por pessoa e por categoria.

O projeto foi separado de forma a permitir futura integração com um **front-end em React + TypeScript**.

---

## 🛠 Tecnologias Utilizadas

- **C#**
- **.NET 8 – ASP.NET Core Web API**
- **Entity Framework Core**
- **SQLite** (persistência local)
- **Swagger (OpenAPI)** para documentação e testes da API

---

## 📁 Estrutura do Projeto

ControleGastos.Api
│
├── Controllers
│ ├── PessoasController.cs
│ ├── CategoriasController.cs
│ ├── TransacoesController.cs
│ └── RelatoriosController.cs
│
├── Domain
│ ├── Entities
│ │ ├── Pessoa.cs
│ │ ├── Categoria.cs
│ │ └── Transacao.cs
│ │
│ └── Enums
│ ├── TipoTransacao.cs
│ └── FinalidadeCategoria.cs
│
├── Dtos
│ ├── CreateTransacaoDto.cs
│ └── Relatorios
│ ├── TotalPorPessoaDto.cs
│ ├── TotaisPorPessoaDto.cs
│ ├── TotaisPorCategoriaItemDto.cs
│ ├── TotaisPorCategoriaResponseDto.cs
│ └── TotaisGeraisDto.cs
│
├── Infrastructure
│ └── Data
│ └── AppDbContext.cs
│
├── Migrations
│
└── Program.cs

---

## ⚙️ Funcionalidades Implementadas

### 👤 Cadastro de Pessoas

Funcionalidades:
- Criar pessoa
- Listar pessoas
- Deletar pessoa

Regras:
- O identificador é gerado automaticamente
- Ao deletar uma pessoa, **todas as transações associadas são removidas em cascata**

Campos:
- Id
- Nome
- Idade

---

### 🏷 Cadastro de Categorias

Funcionalidades:
- Criar categoria
- Listar categorias

Campos:
- Id
- Descrição
- Finalidade:
  - Despesa
  - Receita
  - Ambas

A finalidade é utilizada para validar o tipo de transação permitido.

---

### 💰 Cadastro de Transações

Funcionalidades:
- Criar transação
- Listar transações

Regras de negócio aplicadas:
- O valor da transação deve ser positivo
- Pessoas **menores de 18 anos** só podem registrar **despesas**
- A categoria deve ser compatível com o tipo da transação:
  - Categorias de despesa não aceitam receitas
  - Categorias de receita não aceitam despesas
  - Categorias com finalidade “ambas” aceitam os dois tipos

Campos:
- Descrição
- Valor
- Tipo (Despesa / Receita)
- Categoria
- Pessoa

Para evitar exposição indevida do domínio, foi utilizado um **DTO específico (`CreateTransacaoDto`)** no cadastro de transações.

---

## 📊 Relatórios

### 📌 Totais por Pessoa

Endpoint que retorna:
- Total de receitas por pessoa
- Total de despesas por pessoa
- Saldo (receitas − despesas) por pessoa
- Total geral consolidado de todas as pessoas

---

### 📌 Totais por Categoria

Endpoint que retorna:
- Total de receitas por categoria
- Total de despesas por categoria
- Saldo por categoria
- Total geral consolidado

> Para evitar erros de serialização e ciclos de referência, os relatórios utilizam **DTOs próprios**, sem retornar entidades completas.

---

## 🧪 Testes e Documentação da API

Todos os endpoints podem ser testados diretamente via **Swagger**, disponível em ambiente de desenvolvimento:
http://localhost:5250/swagger/index.html

O Swagger foi utilizado durante todo o desenvolvimento para validar regras de negócio e fluxos da aplicação.

---

## 🗄 Persistência de Dados

- Banco de dados **SQLite**
- Os dados permanecem armazenados mesmo após reiniciar a aplicação
- Migrations gerenciadas via Entity Framework Core

---

## 📌 Decisões Técnicas e Boas Práticas

- Uso de `AsNoTracking()` em operações de leitura para melhorar performance
- Uso de `Include()` apenas quando necessário
- Separação clara entre:
  - Entidades de domínio
  - DTOs de entrada e saída
- Regras de negócio centralizadas nos controllers
- Código comentado para facilitar o entendimento do fluxo e das decisões tomadas
- Tratamento de erros e validações retornando mensagens claras para o consumidor da API

---

## 🚀 Próximos Passos

- Implementação do front-end em **React + TypeScript**
- Melhoria visual e filtros nos relatórios
- Paginação e ordenação nos endpoints de listagem

---

## 👩‍💻 Autoria

Projeto desenvolvido por **Andriele Alves** com foco em .NET, regras de negócio, APIs REST e organização de código.
