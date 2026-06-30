# 08 — Registro de Testes de Usabilidade

## Metodologia

Os testes de usabilidade são aplicados com um grupo restrito de membros reais **antes do lançamento oficial** do sistema.

### Perfil dos Participantes
- **Quantidade:** 5 membros
- **Critério de seleção:** Diferentes faixas etárias, representando o público real da igreja
- **Abordagem:** Observação não-interventiva — o avaliador não interfere durante a execução das tarefas

### Tarefas Solicitadas
1. Criar uma conta no sistema
2. Ativar o Modo Escuro (Dark Mode)
3. Encontrar a data de validade da credencial (carteirinha)

---

## Resultados e Ações Corretivas

### UT-01 — Formulário de Cadastro

| Campo | Detalhe |
|---|---|
| **Tarefa** | Preencher o formulário de cadastro |
| **Dificuldade Identificada** | O navegador preenchida automaticamente o campo de RG com o e-mail salvo |
| **Causa Raiz** | Atributo `autocomplete="off"` ignorado por alguns navegadores modernos |
| **Ação Corretiva** | Substituição de `autocomplete="off"` por `autocomplete="nope"` no HTML |
| **Status** | ✅ Resolvido |

```html
<!-- Antes -->
<input type="text" name="CPF" autocomplete="off" />

<!-- Depois -->
<input type="text" name="CPF" autocomplete="nope" />
```

---

### UT-02 — Dark Mode

| Campo | Detalhe |
|---|---|
| **Tarefa** | Ativar o layout Dark Mode |
| **Dificuldade Identificada** | Ao atualizar a página, o fundo voltava ao tema claro (branco) |
| **Causa Raiz** | Preferência do Dark Mode não era persistida entre sessões |
| **Ação Corretiva** | Implementação de `localStorage` no `site.js` para memorizar a preferência |
| **Status** | ✅ Resolvido |

```javascript
// site.js — Persistência do Dark Mode
document.addEventListener('DOMContentLoaded', () => {
    if (localStorage.getItem('darkMode') === 'enabled') {
        document.body.classList.add('dark-mode');
    }
});
```

---

### UT-03 — Leitura do QR Code

| Campo | Detalhe |
|---|---|
| **Tarefa** | Encontrar a validade do cartão / apresentar QR Code |
| **Dificuldade Identificada** | QR Code inicial de `70px` estava pequeno demais para alguns telemóveis lerem |
| **Causa Raiz** | Tamanho do canvas subestimado para câmeras de menor resolução |
| **Ação Corretiva** | Aumento do canvas do QR Code para `100px` e centralização do layout |
| **Status** | ✅ Resolvido |

```javascript
// Antes
QRCode.toCanvas(canvas, data, { width: 70 });

// Depois
QRCode.toCanvas(canvas, data, { width: 100, margin: 2 });
```

---

## Lições Aprendidas

1. **Autocomplete do navegador** é mais persistente do que `autocomplete="off"` sugere — sempre testar em múltiplos navegadores (Chrome, Safari, Firefox)
2. **Persistência de estado** (Dark Mode, preferências) deve ser planejada desde o início, não tratada como detalhe
3. **Testes com usuários reais** revelam problemas que jamais seriam detectados pelo desenvolvedor — o QR Code pequeno é um exemplo claro disso
4. **Faixas etárias diversas** nos testes garantem que a interface seja acessível para todos os perfis da congregação
