﻿using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.SqlServer
{
    class SqlGenerator_OffsetFetch : SqlGenerator
    {
        protected override void BuildLimitSql(DbExpressions.DbSqlQueryExpression exp)
        {
            //order by number offset 10 rows fetch next 20 rows only;
            this._sqlBuilder.Append("SELECT ");

            List<DbColumnSegment> columns = exp.ColumnSegments;
            for (int i = 0; i < columns.Count; i++)
            {
                DbColumnSegment column = columns[i];
                if (i > 0)
                    this._sqlBuilder.Append(",");

                this.AppendColumnSegment(column);
            }

            this._sqlBuilder.Append(" FROM ");
            exp.Table.Accept(this);
            this.BuildWhereState(exp.Condition);
            this.BuildGroupState(exp);

            List<DbOrderSegment> orderSegs = exp.OrderSegments;
            if (orderSegs.Count == 0)
            {
                DbExpression orderExp = DbExpression.Add(UtilConstants.DbParameter_1, DbConstantExpression.Zero, DbConstantExpression.Zero.Type, null);
                DbOrderSegment orderSeg = new DbOrderSegment(orderExp, OrderType.Asc);
                orderSegs = new List<DbOrderSegment>(1);
                orderSegs.Add(orderSeg);
            }

            this.BuildOrderState(orderSegs);

            this._sqlBuilder.Append(" OFFSET ", exp.SkipCount.Value.ToString(), " ROWS");
            if (exp.TakeCount != null)
            {
                this._sqlBuilder.Append(" FETCH NEXT ", exp.TakeCount.Value.ToString(), " ROWS ONLY");
            }
        }
    }
}
