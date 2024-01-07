using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] GameObject world;
    [SerializeField] GameObject Player;
    [SerializeField] ObjInf objInf;
    [SerializeField] private float generateSize;
    [SerializeField] List<GenPar> genPars;
    [SerializeField] private float chunkSize;
    private float chunkXamount;

    [System.Serializable]
    public class GenPar
    {
        public string name;
        public int[] types;

        public Vector2 rotate;
        public Vector2 scale;
        public int tilingMul;
        [Range(0,10)]public float Amount;
        [Range(1, 30)] public int perlinLayers;
        [Range(0, 2)] public float perlinDestorton;
        [Range(0, 8)] public float ranChanc;
        public float seed;       
        public bool[,] AreChunks;
        public float maxHight;
    } 
    void Start()
    {
        chunkXamount = objInf.worldSize / chunkSize;
        for (int i = 0; i < genPars.Count; i++)
        {
            genPars[i].AreChunks = new bool[(int)chunkXamount, (int)chunkXamount];
            genPars[i].maxHight = 1;
            float n = 1;
            for (int v = 0; v < genPars[i].perlinLayers; v++)
            {

                genPars[i].maxHight += n;
                n /= 2;
            }
        }
        StartCoroutine(GenerateWold());
    }
    private IEnumerator GenerateWold()
    {
        yield return null;

        while (true)
        {
            Vector2 playerpos = new Vector2(Mathf.Round(Player.transform.position.x / chunkSize) * chunkSize, Mathf.Round(Player.transform.position.z / chunkSize) * chunkSize);
            float GenerateSize = Mathf.Round(generateSize / 2 / objInf.tileSize) * objInf.tileSize;
            foreach (GenPar genPar in genPars)
            {//////////////////////////////////////////////////////////////////////////////////////chuks
                for (float x = -GenerateSize; x < GenerateSize; x += chunkSize)
                {
                    for (float z = -GenerateSize; z < GenerateSize; z += chunkSize)
                    {
                        Vector2 chunkpos = new Vector2(playerpos.x + x, playerpos.y + z);

                        int xInChunkArray = Mathf.RoundToInt(chunkpos.x / chunkSize + chunkXamount / 2);
                        int yInChunkArray = Mathf.RoundToInt(chunkpos.y / chunkSize + chunkXamount / 2);
                        
                        
                        yield return null;
                        
                        if (genPar.AreChunks[xInChunkArray, yInChunkArray] == false)
                        {
                            genPar.AreChunks[xInChunkArray, yInChunkArray] = true;
                            /////////////////////////////////////////////////////////////////////////////////////generation in chunks

                            float GenerateSizeinChunk = chunkSize + objInf.tileSize;
                            Vector2 ChunkPosRou = new Vector2(Mathf.Round(chunkpos.x / objInf.tileSize) * objInf.tileSize, Mathf.Round(chunkpos.y / objInf.tileSize)* objInf.tileSize);
                            for (float x2 = -GenerateSizeinChunk; x2 < GenerateSizeinChunk; x2 += objInf.tileSize* (float)genPar.tilingMul)
                            {
                                for (float z2 = -GenerateSizeinChunk; z2 < GenerateSizeinChunk; z2 += objInf.tileSize * (float)genPar.tilingMul)
                                {
                                    Vector2 pos = new Vector2(ChunkPosRou.x + x2, ChunkPosRou.y + z2);
                                    float perlin = CalculetadPerlinNoise(pos, genPar.perlinLayers, genPar.perlinDestorton, genPar.ranChanc, genPar.seed );
                                  
                                    if (genPar.Amount > perlin)
                                    {
                                        int xInArray = Mathf.RoundToInt(pos.x / objInf.tileSize + objInf.worldSize / 2);
                                        int yInArray = Mathf.RoundToInt(pos.y / objInf.tileSize + objInf.worldSize / 2);
                                        if (objInf.PositionInf[xInArray, yInArray] == null)
                                        {
                                            int whatToCopy = Mathf.RoundToInt((Mathf.Clamp( genPar.Amount - perlin ,0, genPar.Amount)/ genPar.Amount) * (float)genPar.types.Length);
                                            GameObject Obj = Instantiate(objInf.TypeInf[genPar.types[Mathf.Clamp(whatToCopy, 0 , genPar.types.Length-1) ]]);
                                            Obj.transform.position = new Vector3(pos.x, 0, pos.y);
                                            Obj.transform.parent = world.transform;
                                            objInf.PositionInf[xInArray, yInArray] = Obj;
                                            
                                        }
                                    }
                                    

                                }

                            }
                            //
                        }     

                    }

                }

                
            }
            yield return null;
        }
    }
    float CalculetadPerlinNoise(Vector2 Point, float perlinNoiseLayers, float perlinDestortion, float ranChance, float seed )
    {
        Vector2 PointChanged = Point+ new Vector2(seed, seed);
        float FinalperlinNoise = 0;
        for (int i = 1; i < perlinNoiseLayers; i++)
        {
            Vector2 calculatedVector = PointChanged * i * perlinDestortion;
            FinalperlinNoise +=  Mathf.PerlinNoise(calculatedVector.x , calculatedVector.y) / i;
            
        }
        float per = Mathf.PerlinNoise(Point.x / 3, Point.y / 3);
        float isItRealy = Mathf.Floor(per * ranChance ) * 1000000f;
        return (FinalperlinNoise + isItRealy);//+ isItRealy;
    }
}
