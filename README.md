# RE4-PS2-BIN-TOOL
Extract and repack RE4 PS2 BIN files

**Translate from Portuguese Brazil**

Esse é o código resultado a partir das pesquisas sobre os arquivos BIN do re4 de ps2, realizadas por Jaderlink em conjunto com o HardRain.
<br>Pode considerar que o programa está na fase final de desenvolvimento em questão de usabilidade. Você pode editar todos os BIN do jogo, com suporte aos arquivos OBJ, SMD (suporte a pesos por bone) e também ele reconhece os IDs das texturas no arquivo MTL;

## Updates

**Update: V.1.5.0**
<br> Melhorado a velocidade do repack, para modelos que têm mais de um material.
<br> Trocado o arquivo "idxbin" pelo "idxps2bin", no qual seu conteúdo é mais fácil de editar.
<br> Agora o conteúdo do 'BoundingBox' é calculado automaticamente quando a tag "AutoCalcBoundingBox" no arquivo idxps2bin estiver como 'true';
<br> Ao fazer repack vai mostrar a ordem em que os materiais/meshes foram colocados no arquivo BIN, isso é usado para conferir se os materiais com transparência estão no final.
<br> Corrigido o campo 'intensity_specular' que estava invertendo os canais Red e Blue no repack com MTL.
<br> Foram feitas melhorias no código.

## RE4_PS2_BIN_TOOL.exe

Programa destinado tanto para extrair ou recompilar o arquivo BIN;

**Extract:**

Ao passar um arquivo BIN como parâmetro para o programa, o programa vai gerar os seguintes arquivos:
<br>  * .OBJ e .SMD: contêm o conteúdo do modelo 3D.
<br>  * .MTL: arquivo que acompanha o .obj serve para carregar as texturas nos modelos, porém as texturas devem ser extraídas do .TPL, e enumeradas a partir de zero.
<br>   (Nota: Agora esse arquivo é usado no Repack do arquivo BIN)
<br> * .IDXPS2BIN: é o arquivo que contém o conteúdo que não está no .obj ou no .smd, mas é necessário para recompilar o .BIN;
<br> * .IDXMATERIAL: arquivo que contém os materiais dos modelos, esse arquivo pode ser usado ao recompilar o arquivo BIN;

 **Repack**

Para criar um novo arquivo .BIN a partir do conteúdo do .OBJ ou .SMD
<br>  - O programa recebe como entrada um arquivo .obj ou .smd, e também deve ter um arquivo .idxps2bin e .MTL na mesma pasta com o mesmo nome do arquivo fornecido.
<br> - Para usar o arquivo .idxmaterial, ao invés do arquivo .MTL a opção "UseIdxMaterial" deve ser definida como true no arquivo .idxps2bin.
<br> - No arquivo MTL é onde são definidos quais são os IDs das texturas usadas em cada material;

 ## Materials/Texturas no MTL

Agora, é no arquivo MTL que é onde são definidos quais são os IDs (índices do TPL) das texturas usadas no arquivo BIN;
<br>O nome da textura tem que ser somente um número que é o ID da textura;
<br>O arquivo MTL espera que essas texturas estejam em uma pasta com o mesmo nome do arquivo BIN que foi extraído;
<br>Para extrair as texturas do arquivo TPL use o programa [RE4-PS2-TPL-TOOL](https://github.com/JADERLINK/RE4-PS2-TPL-TOOL) a partir da versão B.1.1;


## Ordem dos bones no arquivo .SMD

Para arrumar a ordem dos ids dos bones nos arquivos smd, depois de serem exportados do blender ou outro software de edição de modelos,<del> usar o programa: GC_GC_Skeleton_Changer.exe (procure o programa no fórum do re4, remod)</del>
<br>Veja: [SMD_BONE_TOOLS](https://github.com/JADERLINK/SMD_BONE_TOOLS)

## Carregando as texturas no arquivo .SMD

No blender para carregar o modelo .SMD com as texturas, em um novo "projeto", importe primeiro o arquivo .obj para ele carregar as texturas, delete o modelo do .obj importado, agora importe o modelo .smd, agora ele será carregado com as texturas.

## Código de terceiro:
[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader):
Encontra-se em "RE4_PS2_BIN_TOOL\\CjClutter.ObjLoader.Loader", código modificado, as modificações podem ser vistas aqui: [link](https://github.com/JADERLINK/ObjLoader).

**At.te: JADERLINK**
<br>Thanks to "HardRain"
<br>Material information by "Albert"
<br>2025-10-18