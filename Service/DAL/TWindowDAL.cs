﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowDAL
    {
        DbContext db;
        public TWindowDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TWindowDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TWindowModel> GetModelList()
        {
            return db.Query<TWindowModel>().ToList();
        }

        public List<TWindowModel> GetModelList(Expression<Func<TWindowModel, bool>> predicate)
        {
            return db.Query<TWindowModel>().Where(predicate).ToList();
        }

        public TWindowModel GetModel(int id)
        {
            return db.Query<TWindowModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TWindowModel GetModel(Expression<Func<TWindowModel, bool>> predicate)
        {
            return db.Query<TWindowModel>().Where(predicate).FirstOrDefault();
        }

        public TWindowModel Insert(TWindowModel model)
        {
            return db.Insert(model);
        }

        public int Update(TWindowModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TWindowModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_window AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            var dic = new TDictionaryDAL(this.db).GetModelQuery(DictionaryString.WorkState);
            return this.db.Query<TWindowModel>()
                .LeftJoin(dic, (w, d) => w.State == d.Value)
                .LeftJoin<TWindowAreaModel>((w, d, a) => w.AreaName == a.id)
                .Select((w, d, a) => new
                {
                    w.ID,
                    w.Name,
                    w.Number,
                    w.Type,
                    State = d.Name,
                    CallNumber = w.CallNumber,
                    AreaName = a.areaName,
                    Model = w
                })
                .OrderBy(k => k.ID)
                .ToList();
        }
    }
}