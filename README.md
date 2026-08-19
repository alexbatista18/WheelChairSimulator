# WheelChair Simulator

## Autoria

Este projeto é de autoria do **IINN-ELS — Instituto Internacional de Neurociências de Natal Edmond e Lily Safra** (Natal/Macaíba-RN), instituição de pesquisa em neurociência fundada por Miguel Nicolelis, Sidarta Ribeiro e Claudio Mello.

**Alex Batista** ([@alexbatista18](https://github.com/alexbatista18)) atua como **facilitador técnico** do repositório — não é o autor/pesquisador responsável pelo projeto. Para questões de pesquisa, acesso ao hardware físico ou continuidade institucional do projeto, procurar **Edgard Morya** (coordenador de pesquisa no IINN-ELS).

## Pitch

Simulador de cadeira de rodas para PC, desenvolvido em Unity. O jogador controla uma cadeira de rodas virtual através de um dispositivo físico real (a própria cadeira de rodas, instrumentada) conectado ao computador via **Bluetooth**. O dispositivo possui **2 encoders**, um em cada roda, cujos sinais de rotação são lidos pelo Unity e traduzidos em movimento (andar para frente/trás e girar) do avatar no jogo. A ideia é que o simulador seja **multiplayer**, permitindo que mais de um dispositivo/cadeira física seja conectado simultaneamente.

## Hardware

- Dispositivo com 2 encoders (roda direita / roda esquerda), acoplado à cadeira de rodas.
- Comunicação por **Bluetooth**, mas do lado do PC ele aparece como uma **porta serial (COM)** — não como um device Bluetooth "nativo" no Unity.
- **A porta COM não é fixa**: muda de acordo com o notebook/PC usado e com a ordem de pareamento Bluetooth do sistema operacional. É preciso conferir a porta correta no Gerenciador de Dispositivos do Windows e ajustá-la manualmente antes de rodar o simulador (veja "Como rodar" abaixo).
- O hardware necessário (cadeira instrumentada, encoders, etc.) está disponível no **IINN-ELS** (Instituto Internacional de Neurociências de Natal Edmond e Lily Safra). Para acesso ao hardware ou mais informações sobre o dispositivo físico, procurar **Edgard Morya**.

## Estado atual do projeto

O código de leitura da porta serial está em mais de um script, de forma experimental — isso é importante para quem for continuar o projeto:

| Script | Porta/baud (hardcoded) | Formato de dado esperado | Observação |
|---|---|---|---|
| [`Assets/Scripts/MovementSuport.cs`](Assets/Scripts/MovementSuport.cs) (classe `PlayerMovement`) | `COM8` @ 115200 | JSON `{"direita":["v"], "esquerda":["v"]}` (velocidade de cada roda, vinda dos encoders) | Integrado ao Photon (`MonoBehaviourPun`), é o mais próximo do modelo final: usa a diferença de velocidade entre as rodas para andar/girar, com suavização (lerp) de aceleração/desaceleração. |
| [`Assets/Scripts/MovementJoystick.cs`](Assets/Scripts/MovementJoystick.cs) | `COM10` @ 9600 | texto `"x,y"` separado por vírgula | Também integrado ao Photon, mas trata a entrada como um joystick analógico (posição x/y) em vez de velocidade de encoder. Sincroniza posição via RPC (`SyncMovement`). |
| [`Assets/Resources/Avatar/Script/WheelchairControllerBluetooth.cs`](Assets/Resources/Avatar/Script/WheelchairControllerBluetooth.cs) | não abre porta serial própria — recebe dados via `ProcessBluetoothData(string)` chamado externamente | texto `"x,y"` separado por vírgula | Não está conectado ao Photon; parece uma versão anterior/alternativa de leitura de input. |

**Antes de continuar o desenvolvimento**, vale decidir qual desses três caminhos é o "oficial" e remover ou consolidar os outros — hoje eles coexistem no projeto e isso pode confundir. O caminho mais completo (Photon + leitura de encoder por velocidade) é o de `MovementSuport.cs`.

## Multiplayer

- Implementado com **Photon PUN 2** (pasta `Assets/Photon`).
- [`Assets/Scripts/NetworkManager.cs`](Assets/Scripts/NetworkManager.cs) conecta ao Photon Master Server, entra em uma sala fixa chamada `"MyRoom"` (`StartHost` / `StartClient`) com `MaxPlayers = 4`, e já tem 4 posições/rotações de spawn pré-definidas — ou seja, a estrutura para até 4 jogadores/dispositivos simultâneos já existe.
- **Para usar mais de um dispositivo físico ao mesmo tempo**: cada jogador precisa da sua própria porta COM configurada no campo `portName` do script de movimento (público, editável no Inspector por instância/prefab). Isso ainda não tem uma UI de configuração — hoje é preciso trocar o valor manualmente no Editor/prefab para cada máquina/dispositivo antes do build.
- Não há (ainda) UI de lobby para criar/entrar em salas com nomes diferentes — a sala é fixa via código.

## Como rodar (estado atual)

1. Parear o dispositivo Bluetooth da cadeira de rodas com o PC.
2. Verificar no Gerenciador de Dispositivos (Windows) qual porta COM foi atribuída ao dispositivo.
3. Ajustar o campo `portName` no script de movimento usado (ex.: `MovementSuport.cs`) para essa porta, no Inspector do Unity.
4. Abrir o projeto no Unity (ver versão em `ProjectSettings/ProjectVersion.txt`) e rodar a cena principal.
5. Host cria a sala (`StartHost`), demais jogadores entram (`StartClient`).

## Próximos passos sugeridos

- Unificar os três scripts de leitura de input em um único componente configurável (protocolo de dados + porta/baud).
- Criar uma tela simples para selecionar a porta COM em vez de hardcode, facilitando trocar de máquina.
- Testar o fluxo com mais de 2 dispositivos físicos simultâneos (estrutura de sala já suporta até 4 jogadores).
- Avaliar se o pareamento Bluetooth pode ser automatizado/detectado (hoje depende de configuração manual do SO).

## Contato

- **Facilitador (repositório/código):** Alex Batista — (69) 99369-7356 — dkalexbatista@gmail.com ([@alexbatista18](https://github.com/alexbatista18))
- **Autoria/pesquisa:** IINN-ELS — Instituto Internacional de Neurociências de Natal Edmond e Lily Safra.
- **Hardware** (cadeira instrumentada, encoders): disponível no IINN-ELS. Para mais informações, procurar **Edgard Morya**.

## Referências

- [Photon PUN 2 (Unity)](https://doc.photonengine.com/pun/current/getting-started/pun-intro)
