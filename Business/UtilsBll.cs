using DBUtility;
using System;
using System.Data;
using System.Text;

namespace Business
{
    public class UtilsBll
    {
        /// <summary>
        /// 首页统计信息
        /// </summary>
        /// <returns></returns>
        public DataSet GetIndexReport()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("SELECT * FROM ");

            strSql.Append("(SELECT COUNT(*) as petition_count FROM ci_petition WHERE ISDELETE = 0 ) t1, ");

            strSql.Append("(SELECT COUNT(*) as finish_count FROM ci_petition WHERE ISDELETE=0 AND STATUS = 1 ) t2,");

            strSql.Append("(SELECT COUNT(*) as user_count FROM ci_admin WHERE ISDELETE <> 1 ) t3,");

            strSql.Append("(SELECT COUNT(*) as notfinish_count FROM ci_petition WHERE ISDELETE=0 AND STATUS = 0 ) t4,");

            strSql.Append("(SELECT COUNT(*) as overdue_count FROM (" + GetOverduePetition() + ") b ) t5");

            //strSql.Append("(SELECT SUM(HASTIMES) as act_count FROM ci_device  ) t4");

            return SqlHelper.Query(strSql.ToString());
        }

        public StringBuilder GetWarnPetitionSql()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("select ID,DATE_FORMAT( CREATEDATE, '%Y-%m-%d' ) AS CREATEDATE,PNAME,");
            strSql.Append("pIdCard,pAddress,status,modifyTime,CASETYPE,CASENAME,");
            strSql.Append("CASESOURCE,CHANNELS,RECEIVER,RERM,EXT1,EXT2,EXT3,EXT4,EXT5,DATE_FORMAT( adddate(CREATEDATE,RERM), '%Y-%m-%d' ) as RERMDATE,DATEDIFF(adddate(CREATEDATE,RERM),NOW()) AS WARNING");
            strSql.Append(" FROM ci_petition ");
            strSql.Append(" where ISDELETE = 0 AND `STATUS` = 0 ");
            strSql.Append(" ORDER BY WARNING");

            return strSql;
        }

        /// <summary>
        /// 过期案件
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetOverduePetition()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT ID,CREATEDATE,PNAME,pIdCard,pAddress,status,modifyTime,CASETYPE,CASENAME,");
            strSql.Append("CASESOURCE,CHANNELS,RECEIVER,RERM,EXT1,EXT2,EXT3,EXT4,EXT5,RERMDATE,WARNING ");
            strSql.Append(" FROM (" + GetWarnPetitionSql() + ") a");
            strSql.Append(" WHERE WARNING < 0");
            return strSql;
        }

        /// <summary>
        /// 查询折线图数据
        /// </summary>
        /// <returns></returns>
        public DataSet ActIndexReport()
        {
            string dateTime = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT DATE_FORMAT(CREATEDATE, '%Y-%m-%d') AS time, SUM(1) AS total ");
            strSql.Append("FROM ci_petition WHERE ISDELETE = 0 and CREATEDATE > '" + dateTime + "' ");
            strSql.Append(" GROUP BY DATE_FORMAT(CREATEDATE,'%Y-%m-%d') ");
            return SqlHelper.Query(strSql.ToString());
        }

    }
}
