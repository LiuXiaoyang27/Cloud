using Business;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Controller
{
    public class OldPetitionController
    {
        private readonly OldPetitionBll dal;

        public OldPetitionController()
        {
            dal = new OldPetitionBll();
        }
      

        public DataSet GetList()
        {
            return dal.GetList();
        }



        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public string Add(OldPetition Model)
        {
            return dal.Add(Model);
        }

    }
}
