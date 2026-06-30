# Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato segue [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/).

---

## [Não lançado]

### Adicionado
- Estrutura inicial do projeto ASP.NET Core MVC
- Entidades de domínio: `Usuario`, `Evento`, `Presenca`, `Justificativa`
- Migrations iniciais do Entity Framework Core
- Módulo de Autenticação com validação de código para perfis administrativos
- Carteirinha digital com QR Code dinâmico (canvas 100px)
- Check-in por geolocalização (GPS) com formulário oculto POST
- Alerta pastoral automático via consulta LINQ (3 faltas consecutivas)
- Dark Mode global com persistência via `localStorage`
- Responsividade mobile-first (`max-width: 414px`, Flexbox)

### Corrigido
- `autocomplete="nope"` no formulário de cadastro para evitar preenchimento indevido pelo navegador
- Persistência do Dark Mode ao navegar entre páginas
- Tamanho do QR Code aumentado de 70px para 100px após testes de usabilidade

---

## Como contribuir com o changelog

Ao abrir um Pull Request, inclua a descrição da mudança neste arquivo sob a seção `[Não lançado]` na categoria adequada:

- **Adicionado** — para novas funcionalidades
- **Alterado** — para mudanças em funcionalidades existentes
- **Descontinuado** — para funcionalidades que serão removidas
- **Removido** — para funcionalidades removidas
- **Corrigido** — para correções de bugs
- **Segurança** — para vulnerabilidades corrigidas
