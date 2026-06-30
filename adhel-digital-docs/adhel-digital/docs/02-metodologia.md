# 02 — Metodologia de Desenvolvimento

## Abordagem

O desenvolvimento seguiu uma **abordagem ágil simplificada**, focada em entregas contínuas e modulares organizadas em iterações.

## Fases do Processo

### 1. Levantamento de Requisitos
Definição das regras de negócio, incluindo:
- Fluxo de aprovação de cadastros pendentes
- Lógica de alertas pastorais (ex: ausência em 3 Santas Ceias consecutivas)
- Regras de acesso por perfil (Membro, Secretaria, Pastor)

### 2. Desenvolvimento Incremental
Criação do projeto por camadas:
1. Estrutura do Banco de Dados (Entidades e Migrations)
2. Lógica de Backend em C# (Controllers, Services, LINQ)
3. Frontend (Views Razor, HTML/CSS, JavaScript)

## Ferramentas Utilizadas

| Categoria | Ferramenta |
|---|---|
| **IDE** | Visual Studio |
| **Linguagem** | C# (ASP.NET Core MVC) |
| **Frontend** | HTML5, CSS3, JavaScript puro |
| **Ícones** | FontAwesome |
| **Banco de Dados** | SQL Server |
| **ORM** | Entity Framework Core |

## Vantagens da Abordagem Modular

- Facilita a identificação e correção de erros em cada camada
- Permite testes unitários por módulo antes da integração
- Agiliza a adaptação a novos requisitos surgidos durante o desenvolvimento
