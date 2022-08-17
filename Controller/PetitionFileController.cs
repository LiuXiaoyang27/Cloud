using Business;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    /// <summary>
    /// 信访案件文件操作类 
    /// </summary>
    public class PetitionFileController
    {
        private readonly PetitionFileBll dal;
        public PetitionFileController()
        {
            dal = new PetitionFileBll();
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(string petitionId)
        {
            string strWhere = " isDelete = 0 and petitionId = " + petitionId;
            return dal.GetList(strWhere);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public string Add(PetitionFiles model)
        {
            return dal.Add(model);
        }

        /// <summary>
        ///  修改信息
        /// </summary>
        public bool Update(string petitionId, List<PetitionFiles> list)
        {
            PetitionFiles model;
            string deleteIds = "";
            string updateIds = "";
            int status = 0;
            for (int i = 0; i < list.Count; i++)
            {
                model = list[i];
                status = model.status;
                if (status == 1)
                {
                    if (string.IsNullOrEmpty(deleteIds))
                    {
                        deleteIds = "'" + model.id + "'";
                    }
                    else
                    {
                        deleteIds += ",'" + model.id + "'";
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(updateIds))
                    {
                        updateIds = "'" + model.id + "'";
                    }
                    else
                    {
                        updateIds += ",'" + model.id + "'";
                    }
                }
            }
            if (!string.IsNullOrEmpty(deleteIds))
            {
                dal.Update(petitionId, 1, deleteIds);
            }
            if (!string.IsNullOrEmpty(updateIds))
            {
                dal.Update(petitionId, 0, updateIds);
            }
            return true;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public bool Delete(string ids)
        {
            return dal.Delete(ids);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteBatch(List<string> ids)
        {
            return dal.DeleteBatch(ids);
        }
    }
}
