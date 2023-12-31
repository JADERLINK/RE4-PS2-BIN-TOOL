# RE4-PS2-BIN-TOOL
Extract and repack ps2 re4 bin files

**Translate from Portuguese Brazil**

Esse é o código resultado a partir das pesquisas sobre os arquivos .bin do re4 de ps2, realizadas por Jaderlink em conjunto com o HardRain.
<br>Pode considerar que o programa está na fase final de desenvolvimento em questão de usabilidade. Você pode editar todos BIN do jogo, com suporte aos arquivos OBJ, SMD (suporte a pesos por bone) e também ele reconhece os IDs das texturas no arquivo MTL;

## Update: B.1.4.0.0

Agora o "Extract" e "Repack" são feitos somente por um programa, melhorei como os índices das texturas são atribuídos ao modelo, que pode ser feito usando o arquivo MTL, ou o arquivo "ixdmaterial";
<br>Os nomes dos "Bones" agora recebem como nome o ID do "Bone";

## RE4_PS2_BIN_TOOL.exe

Programa destinado tanto para extrair ou recompilar o arquivo BIN;

**Extract:**

Ao passar um arquivo BIN como parâmetro para o programa, o programa vai gerar os seguintes arquivos:
<br>  * .OBJ e .SMD: contem o conteúdo do modelo 3d.
<br>  * .MTL: arquivo que acompanha o .obj serve para carregar as texturas nos modelos, porém as texturas devem ser extraídas do .TPL, e enumeradas a partir de zero.
<br>   (Nota: Agora esse arquivo é usado no Repack do arquivo BIN)
<br> * .IDXBIN: é o arquivo que contém o conteúdo que não está no .obj ou no .smd, mas é necessário para recompilar o .BIN;
<br> * .IDXMATERIAL: arquivo que contém os materiais dos modelos, esse arquivo pode ser usado ao recompilar o arquivo BIN;

 **Repack**

Para criar um novo arquivo .BIN a partir do conteúdo do .OBJ ou .SMD
<br>  - O programa recebe como entrada um aquivo .obj ou .smd, e também deve ter um arquivo .idxbin e .MTL na mesma pasta com o mesmo nome do arquivo fornecido.
<br> - Caso você passe como segundo parâmetro um arquivo .idxmaterial, esse arquivo vai ser usado ao invés do arquivo .MTL.
<br> - No arquivo MTL é onde são definidos quais são os IDs das texturas usadas em cada material;

 ## Materials/Texturas no MTL

Agora, é no arquivo MTL é onde são definidos quais são os IDs (índices do TPL) das texturas usadas no arquivo BIN;
<br>Agora, o nome da textura tem que ser somente um número que é o ID da textura;
<br>O arquivo MTL espera que essas texturas estejam em uma pasta como o mesmo nome do arquivo BIN que foi extraído;
<br>Para o extrair as texturas do arquivo TPL use o programa [RE4-PS2-TPL-TOOL](https://github.com/JADERLINK/RE4-PS2-TPL-TOOL) a partir da versão B.1.1.0.0;


## Ordem dos bones no arquivo .SMD

Para arrumar a ordem dos ids dos bones nos arquivos smd, depois de serem exportados do blender ou outro software de edição de modelos usar o programa: GC_GC_Skeleton_Changer.exe (procure o programa no fórum do re4)

## Carregando as texturas no arquivo .SMD

No blender para carregar o modelo .SMD com as texturas, em um novo "projeto", importe primeiro o arquivo .obj para ele carregar as texturas, delete o modelo do .obj importado, agora importe o modelo .smd, agora ele será carregado com as texturas.

## Lista de BINs incompativeis com o programa:

Segue abaixo a lista com os bins que são incompatíveis com o programa, pois são de outro formato.

* Esse citados abaixo, na verdade são arquivo de game Cube:

```
em41_440.BIN
em41_442.BIN
em41_443.BIN
em41_444.BIN
em41_445.BIN
em41_446.BIN
em41_447.BIN
em41_448.BIN
em41_449.BIN
em41_450.BIN
em41_451.BIN
em41_452.BIN
```
* E esses citados abaixo, são de outro formato desconhecido:
```
pl02_007.BIN
ss_oc101_005.BIN
ss_oc102_005.BIN
ss_oc103a_005.BIN
ss_oc103b_005.BIN
ss_oc104_005.BIN
ss_oc105_005.BIN
ss_oc106_005.BIN
ss_oc107_005.BIN
ss_oc108_005.BIN
ss_oc109_005.BIN
ss_oc110_005.BIN
ss_oc111_005.BIN
ss_oc112_005.BIN
ss_oc201_005.BIN
ss_oc202_005.BIN
ss_oc203_005.BIN
ss_oc204_005.BIN
ss_oc205_005.BIN
ss_oc206_005.BIN
ss_oc301_005.BIN
ss_oc302_005.BIN
ss_oc303_005.BIN
ss_oc304_005.BIN
ss_oc305_005.BIN
ss_term_008.BIN
```

## Código de terceiro:

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader):
Encontra-se no RE4_PS2_BIN_TOOL, código modificado, as modificações podem ser vistas aqui: [link](https://github.com/JADERLINK/ObjLoader).

**At.te: JADERLINK**
<br>2023-12-31