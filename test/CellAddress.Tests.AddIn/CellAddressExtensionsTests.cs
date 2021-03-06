﻿using System;
using System.Linq;
using ExcelDna;
using ExcelDna.Extensions;
using ExcelDna.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CellAddressTests.AddIn {
    [TestClass]
    public class CellAddressExtensionsTests
    {
        [TestMethod]
        public void TestClearContents() {
            const string msg = "Test Clear Contents";
            var cell = CellAddress.Parse("A1");
            cell.SetValue(msg);
            Assert.AreEqual(msg, cell.GetValue<string>());

            cell.ClearContents();
            Assert.IsTrue(cell.GetValue<object>().IsNull());

            //清理多个单元格内容
            cell = CellAddress.Parse("A1:A5");
            foreach (var c in cell.GetCells()) {
                c.SetValue(msg);
            }
            cell.ClearContents();
            Assert.IsTrue(cell.GetValues<object>().All(v=>v.IsNull()));
        }

        [TestMethod]
        public void TestGetCellWithIndex() {
            var cells = CellAddress.Parse("A1:F5");
            var cell = cells.GetCell(1,XlFillDirection.ColumnFirst);
            Assert.AreEqual("$B$1", cell.LocalAddress);

            var cell1 = cells.GetCell(2, XlFillDirection.RowFirst);
            Assert.AreEqual("$A$3", cell1.LocalAddress);
        }

        [TestMethod]
        public void TestOffset() {
            var cell = CellAddress.Parse("C4");
            Assert.AreEqual("$C$5", cell.Offset(1).LocalAddress);

            Assert.AreEqual("$D$4", cell.Offset(0,1).LocalAddress);

            Assert.AreEqual("$D$5", cell.Offset(1, 1).LocalAddress);

            Assert.AreEqual("$A$3", cell.Offset(-1, -2).LocalAddress);

            Assert.ThrowsException<IndexOutOfRangeException>(() => { cell.Offset(-10); });
        }

        [TestMethod]
        public void TestMax() {
            var c1 = CellAddress.Parse("A1");

            var c2 = CellAddress.Parse("A2");

            Assert.IsTrue(c1.Max(c2)==c2);

        }

        [TestMethod]
        public void TestSetFormula() {
            var value = "Test Formula";
            CellAddress.Parse("A1").SetValue(value);

            var c2 = CellAddress.Parse("B1");
            c2.SetFormula("=A1");

            Assert.AreEqual(value, c2.GetValue<string>());

            ((CellAddress)"A2").SetValue(value);

            var cells = ((CellAddress) "B1:B2");
            cells.SetFormula("=A1");

            Assert.IsTrue(cells.GetCells().All(c => c.GetFormula()=="=A1"));
        }

        [TestMethod]
        public void TestGetRange() {
            var cells = new CellAddress[] {
                "A1", "B2", "D5", "F3"
            };
            Assert.AreEqual("$A$1:$F$5", cells.GetRange().LocalAddress);
        }

        [TestMethod]
        public void TestGetCell() {
            var cell = CellAddress.Parse("Sheet1!A1:B2");
            Assert.AreEqual(4,cell.Count);
            Assert.AreEqual(0, cell.ColumnFirst);
            Assert.AreEqual(0, cell.RowFirst);

            var nextCol = cell.GetCell(1, XlFillDirection.RowFirst);
            Assert.AreEqual("$A$2", nextCol.LocalAddress);

            var nextRow = cell.GetCell(1, XlFillDirection.ColumnFirst);
            Assert.AreEqual("$B$1", nextRow.LocalAddress);
        }

        [TestMethod]
        public void TestNextCell() {
            var cell = CellAddress.Parse("Sheet1!A1:B2");
            Assert.AreEqual(4, cell.Count);

            var nextCol = cell.NextCell(1, XlFillDirection.RowFirst);
            Assert.AreEqual("$A$2", nextCol.LocalAddress);

            var nextRow = cell.NextCell(1, XlFillDirection.ColumnFirst);
            Assert.AreEqual("$B$1", nextRow.LocalAddress);
        }

        [TestMethod]
        public void TestGetCellForColumn() {
            var diect = XlFillDirection.ColumnFirst;
            var range = CellAddress.Parse("Sheet1!A1:F10");
            for (int i = 0; i < range.Count; i++) {
                var cell = range.GetCell(i, diect);
                Console.WriteLine($"{i}:{cell.LocalAddress}");
            }
        }

        [TestMethod]
        public void TestGetCellForRow() {
            var diect = XlFillDirection.RowFirst;
            var range = CellAddress.Parse("Sheet1!A1:F10");
            for (int i = 0; i < range.Count; i++) {
                var cell = range.GetCell(i, diect);
                Console.WriteLine($"{i}:{cell.LocalAddress}");
            }
        }
    }
    /*
    [TestClass]
    public class ExcelReferenceExTests {

        [TestMethod]
        public void TestClearFormula() {
            ((CellAddress)"A1").SetValue("TEST Clear Formula 1#");
            ((CellAddress)"A2").SetValue("TEST Clear Formula 1#");

            var c1 = CellAddress.Parse("B1");
            c1.SetFormula("=A1");

            var c2 = CellAddress.Parse("B2");
            c2.SetFormula("=A2");

            var range = CellAddress.Parse("B1:B2");
            Assert.IsTrue(range.GetCells().All(c=>c.CellReference.HasFormula()));

            //XlCall.Excel(XlCall.xlcFormula, string.Empty, range);
            range.CellReference.ClearFormula();

            Assert.IsTrue(range.GetCells().All(c => !c.CellReference.HasFormula()));

            range.SetFormula("=A1");
            Assert.IsTrue(range.GetCells().All(c => c.CellReference.HasFormula()));
        }
    }
    */
}
