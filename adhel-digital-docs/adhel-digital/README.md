# ADHEL Digital 🏛️

> Sistema de Gestão de Membresia para Igrejas — Aplicação Web Mobile-First

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![Tech](https://img.shields.io/badge/tech-ASP.NET%20Core%20MVC-blue)
![DB](https://img.shields.io/badge/banco-SQL%20Server-red)

---

## 📋 Visão Geral

O **ADHEL Digital** é uma aplicação web focada em dispositivos móveis que digitaliza a gestão de membresia eclesiástica. O sistema substitui carteirinhas físicas por credenciais digitais com QR Code dinâmico, automatiza o check-in em cultos de Santa Ceia via geolocalização (GPS) e fornece um painel administrativo inteligente com alertas pastorais.

### Público-Alvo

| Perfil | Necessidade Principal |
|---|---|
| **Membros** | Acesso rápido à credencial digital, agenda e comunicados |
| **Secretaria** | Aprovação de cadastros e manutenção de dados |
| **Liderança / Pastor** | Métricas e alertas automáticos de cuidado pastoral |

---

## 🚀 Funcionalidades Principais

- **Credencial Digital com QR Code** — Geração dinâmica em JavaScript com validação visual (vermelho para expirado, preto para ativo)
- **Check-in por Geolocalização** — Assinatura de lista via GPS diretamente no navegador
- **Alerta Pastoral Automático** — Detecção de membros com 3 faltas consecutivas em Santas Ceias
- **Dark Mode Global** — Preferência salva via `localStorage` para conforto em ambientes escuros
- **Painel Administrativo** — Dashboard para secretaria com controle de membros pendentes/ativos

---

## 🛠️ Tecnologias Utilizadas

| Camada | Tecnologia |
|---|---|
| **IDE** | Visual Studio |
| **Backend** | C# / ASP.NET Core MVC |
| **Frontend** | HTML5, CSS3, JavaScript puro, FontAwesome |
| **Banco de Dados** | SQL Server via Entity Framework Core (ORM) |
| **Arquitetura** | MVC (Model-View-Controller) + Cliente-Servidor |

---

## 🗂️ Estrutura do Projeto

```
adhel-digital/
├── README.md
├── docs/
│   ├── 01-contexto-e-objetivo.md
│   ├── 02-metodologia.md
│   ├── 03-interface-ui-ux.md
│   ├── 04-arquitetura.md
│   ├── 05-modelo-banco-de-dados.md
│   ├── 06-funcionalidades.md
│   ├── 07-plano-de-testes.md
│   └── 08-testes-de-usabilidade.md
└── CHANGELOG.md
```

---

## ⚙️ Como Executar

```bash
# 1. Clonar o repositório
git clone https://github.com/seu-usuario/adhel-digital.git

# 2. Entrar na pasta do projeto
cd adhel-digital

# 3. Restaurar dependências
dotnet restore

# 4. Aplicar migrations do banco de dados
dotnet ef database update

# 5. Executar a aplicação
dotnet run
```

> **Pré-requisitos:** .NET 8 SDK, SQL Server, Visual Studio 2022+

---

## 📐 Arquitetura

O sistema segue o padrão **MVC** com comunicação HTTP entre cliente e servidor:

```
[ Navegador Mobile ]
        │  HTTP Request
        ▼
[ Controller (C#) ] ──── Regras de Negócio
        │
        ├──► [ Model (.cs) ] ──── Entity Framework Core ──► [ SQL Server ]
        │
        └──► [ View (.cshtml) ] ──── Razor ──── HTML/CSS/JS ──► Navegador
```

---

## 🗃️ Modelo de Dados

| Entidade | Propriedades Principais |
|---|---|
| `Usuario` | Id, Nome, Email, Senha, CPF, Perfil, Status, DataValidade |
| `Evento` | Id, Titulo, DataHora, Tipo |
| `Presenca` | Id, UsuarioId, EventoId, Status (Presente/Falta) |
| `Justificativa` | Id, UsuarioId, EventoId, Motivo |

---

## 🤝 Contribuição

1. Faça um _fork_ do projeto
2. Crie uma branch (`git checkout -b feature/minha-feature`)
3. Commit suas alterações (`git commit -m 'feat: adiciona minha feature'`)
4. Push para a branch (`git push origin feature/minha-feature`)
5. Abra um _Pull Request_

---

## 📄 Licença

Este projeto foi desenvolvido para fins acadêmicos e institucionais. Consulte a equipe ADHEL para informações sobre uso e distribuição.

---

*Desenvolvido com ❤️ para a gestão pastoral moderna.*
