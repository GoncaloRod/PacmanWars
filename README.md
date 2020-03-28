# Pacman Wars

- [Introdução](#Introdução)
- [Controlos](#Controlos)
- [_Game_](#Game)
- [Tabuleiro de Jogo](#Tabuleiro-de-Jogo)
- [Jogador](#Jogador)
- [Enimigos](#Enimigos)
- [Pac-Dots](#Pac-Dots)
- [Power Pellets](#Power-Pellets)
- [Frutas Bonus](#Frutas-Bonus)
- [UI](#UI)

## Introdução

O Pacman Wars é um jogo que se baseia no Pacman Original mas com a introdução de um segundo jogador. Um jogador perde ao ficar sem vidas. O objetivo do jogo é bater o _high score_ porém o mesmo só é valido se o jogador for o vencedor.

## Controlos

O jogador 1 usa as teclas <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd> para se movimentar e o jogador 2 usa as teclas <kbd>Up</kbd> <kbd>Left</kbd> <kbd>Down</kbd> <kbd>Right</kbd>.

Para não ser necessária a criação de 2 classes destintas para cada jogador decidimos passar uma _struct_ com a informção da tecla usada para cada um dos _inputs_ no construtor da classe.


## _Game_

A classe _Game_ começa por carregar para memória o ficheiro com as informações do tabuleiro de jogo e criar e guardar os diferentes componentes do jogo. Tenta também ler o _high score_, se não conseguir, por o ficheiro não existir ou estár vazio, o _high score_ será 0.

A cada _frame_ verifica se ainda existem Pac-Dots e Power Pellets, se já não existir nenhuma os jogadores e enimigos voltam para a sua posição inicial e as Pac-Dots e Power Pellets voltam a ser criadas, ou seja, o nível recomeça mantendo o número de vidas e as pontuações dos jogadores.


## Tabuleiro de Jogo

No início do jogo é carregado um ficheiro com o seguinte formato:

```
WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
WP             PWP             PW
W WWWWWW WWWWWW W WWWWWW WWWWWW W
W   W                       W   W
W W W WWWW WWWWW WWWWW WWWW W W W
W W W   PW WP       PW WP   W W W
W     WWWW W WWWIWWW W WWWW     W
WWWWW        WEESEEW        WWWWW
W     WWWW W WWWWWWW W WWWW     W
W W W   PW WP       PW WP   W W W
W W W WWWW WWWWW WWWWW WWWW W W W
W   W                       W   W
W WWWWWWW WWWWW W WWWWW WWWWWWW W
W E1E           W           E2E W
WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
```

Cada caracter representa o seguinte:
- W - Parede
- I - Parede Invisível
- E - Espaço Vazio
- Espaço vazio - "Pac-Dot"
- P - "Power Pellet"
- 1 - Jogador 1
- 2 - Jogador 2
- S - _Spawn_ de Enimigos

Depois do ficheiro carregado e convertido numa matriz de caracteres essa é analizada usando um algoritmo para gerar uma lista com as informações sobre que posição na _sprite_ usar ao desenhar cada uma das _tiles_.

Para desenhar o tabuleiro no ecrã basta percorrer a lista de _tiles_ e desenha-las na sua posição.


## Jogador

Cada jogador começa o jogo com 0 pontos e com 3 vidas. Um jogador perde quando o seu número de vidas é 0.

Quando um jogador morre fica com um efeito de invencibilidade durante 2 segundos.

A classe do jogador é responsável pelo seu movimento. Esta oferece acesso a algumas propriedades como a sua posição, o retângulo da sua área, pontuação, número de vidas, entre outras, e permite também que outros componentes adicionem pontos á sua pontuação ou matem o _player_.

Quando um jogador perde é disparado um evento para que na classe do _Game_ seja atualizada um propriedade que devolve o jogador vencedor.

O jogador ao atigir 10000 pontos recebe 1 vida extra.


## Enimigos

Existem 4 enimigos no jogo, cada um com a sua propria cor, tal como no jogo original.

A classe do enimigo é responsável por controlar o seu movimento de forma autónoma, verificar interseções com os jogadores e reagir quando uma Power Pellet é apanhada.

O movimento autónomo é feito usando um algoritmo bastante simples. Quando é necessário calcular uma nova posição de destino é feita uma análise a sua volta para descobrir quais são destinos válidos. Com todos os possíveis destinos guardados numa lista, essa lista é ordenada pela distância ao jogador mais próximo, no final o destino com a menor distância vai estar na primeira posição da lista e o destino com a maior distância vai estar no fim da lista. Por último caso o enimigo estaja em mode de fuga, por uma Power Pellet ter sido apanha recentemente, escolhe como posição de destino a que estiver no fim da lista, se não escolhe a que estiver no inicio da lista.

A cada _frame_ o enimigo verifica se a sua área interseta com a área de algum jogador e caso intersete o jogador em questão morre, se o enimigo estiver em modo normal, ou o enimigo morre e o jogador recebe pontos, caso o enimigo estaja a fugir.


## Pac-Dots

As Pac-Dots são os pontos mais pequenos que se encontram espalhados pelo tabuleiro e apenas verificam, a cada _frame_, se a sua área interseta com a de algum jodador. Caso isso seja verdade o jodor em questão recebe pontos. Se ambos os jodares intersetarem a Pac-Dot aquele que tiver menor pontuação recebe os pontos.


## Power Pellets

As Power Pellets apenas diferem das Pac-Dot num aspeto. Quando uma Power Pellet é apanhada por um jogador é disparado um evento que vai fazer com que os enimigos reajam e entrem em modo de fuga tal como no jogo original.


## Frutas Bonus


## UI

Como o nosso jogo tem mais que um jogador e é necessário mostrar as informações de cada um deles no ecrã achamos melhor que o UI fosse desenhado por um componente á parte.

O UI do nosso jogo consiste apenas em mostrar as vidas que cada jogador tem e a sua pontuação atual, o último _High Score_ atingido e, caso o jogo tenha terminado, o número do jogador vencedor.
