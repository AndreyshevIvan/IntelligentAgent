using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelligentAgent
{
    class MapData
    {
        public MapData(string mapJson)
        {
            //  http://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm документация с примером
            JObject JsonGameInfo = JObject.Parse(mapJson);  


            // ИНФОРМАЦИЯ О ПАРСИНГЕ.
            // Идет обращения вглубь вложенности text -> iagent -> knowCaves ->
            // далее приходится брать дочерние элементы, так как каждый Cave(пещера) имеет свой идентификатор (пример: "0_0")
            // после этого требуется перейти на следующую ступень вложенности, так как наша реализация Cave, не подразумевает
            // наличия своего собственного идентификатора.
            // После этого требуется превратить все пещеры в список пещер, для этого преобразуем всё в List()
            // Так как в данный момент все пещеры являются экземплярами класса JToken (не читал, ищи в документации), нам
            // требуется перевести их в экземпляры класса Cave, делается циклом foreach toObject<Cave>, и запихнуть в список List<Cave>
            IList<JToken> JsonKnowCaves = JsonGameInfo["text"]["iagent"]["knowCaves"].Children().Children().ToList();
            knowCaves = new List<Cave>();
            foreach(JToken cave in JsonKnowCaves)
            {
                Cave searchCave = cave.ToObject<Cave>();
                knowCaves.Add(searchCave);
            }
            m_world = JsonGameInfo["text"]["worldinfo"].ToObject<World>();

            // под m_cave я понимаю CurrentCave.
            m_cave = JsonGameInfo["text"]["currentcave"].ToObject<Cave>();
        }
        public bool GetOpenWorld(ref CavesMap cavesMap)
        {
            if (!m_isOpenWorld || !isValid)
            {
                return false;
            }

            cavesMap = new CavesMap(4, 4);
            return true;
        }

        public bool isValid { get { return m_isValid; } }
        public string endLog { get { return m_endLog; } }
        public Cave currentCave { get { return m_cave; } }
        public IList<Cave> knowCaves { get; set; }
        public World currentWorld { get { return m_world; } }

        private bool m_isValid = true;
        private bool m_isOpenWorld = false;
        private string m_endLog = "";
        private Cave m_cave;
        private World m_world;
    }
}
