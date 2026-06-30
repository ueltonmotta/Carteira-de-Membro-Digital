# 03 — Projeto de Interface (UI/UX)

## Princípio de Design

A interface foi desenhada com foco absoluto na **usabilidade móvel**, considerando que os membros acederão ao sistema maioritariamente através de smartphones durante os cultos.

## Conceito Visual

| Atributo | Especificação |
|---|---|
| **Estilo** | Design limpo e institucional |
| **Azul Marinho** | `#0d1326` |
| **Dourado** | `#bba371` |
| **Branco** | `#ffffff` |

## Responsividade

- Utiliza `max-width: 414px` para simular perfeitamente um aplicativo nativo no ecrã do navegador
- Layout construído com **Flexbox** para adaptação fluida a diferentes tamanhos de tela
- Otimizado para uso em modo retrato (portrait)

## Acessibilidade — Modo Escuro (Dark Mode)

O sistema implementa um **Dark Mode global** com as seguintes características:

- **Ativação:** Toggle via JavaScript (botão acessível na interface)
- **Persistência:** Preferência salva no `localStorage` do navegador
- **Benefício:** Conforto visual em ambientes com pouca luz, como durante os cultos noturnos

### Implementação Técnica do Dark Mode

```javascript
// site.js — Verificação ao carregar a página
if (localStorage.getItem('darkMode') === 'enabled') {
    document.body.classList.add('dark-mode');
}

// Toggle ao clicar
function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    const isDark = document.body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDark ? 'enabled' : 'disabled');
}
```

## Credencial Digital

A carteirinha digital é o elemento central da experiência do membro:

- **QR Code Dinâmico:** gerado em JavaScript com canvas de `100px` (tamanho validado em testes de usabilidade)
- **Validação Visual:**
  - 🟢 **Preto:** credencial ativa e válida
  - 🔴 **Vermelho:** credencial expirada
  - ⚠️ **Marca d'água:** status "Aguardando Aprovação" para membros pendentes
