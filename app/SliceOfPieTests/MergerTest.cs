using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SliceOfPie;

namespace SliceOfPieTests {
    [TestClass]
    public class MergerTest {
        Document nullRevDoc = new Document() { CurrentRevision = null };
        Document nullDoc = null;
        Document emptyDoc = new Document() { CurrentRevision = "" };

        //rest of the test documents declared in the bottom of file, because they are butt-ugly

        //Because the two interfaces (String and Document) use the same underlying method, they are tested together here
        //Test for null texts
        [TestMethod] 
        public void BothNullRevDocsTest() {
            Assert.IsNull(Merger.Merge(nullRevDoc, nullRevDoc));
        }

        [TestMethod]
        public void CurrNullRevDocTest() {
            Assert.AreEqual(originalDoc.CurrentRevision, Merger.Merge(nullRevDoc, originalDoc).CurrentRevision);
        }

        [TestMethod]
        public void OldNullRevDocTest() {
            Assert.AreEqual(originalDoc.CurrentRevision, Merger.Merge(originalDoc, nullRevDoc).CurrentRevision);
        }

        //Tests for null documents
        [TestMethod]
        public void BothNullDocTest() {
            Assert.IsNull(Merger.Merge(nullDoc, nullDoc));
        }

        [TestMethod]
        public void CurrNullDocTest() {
            Assert.AreEqual(originalDoc.CurrentRevision, Merger.Merge(null, originalDoc).CurrentRevision);
        }

        [TestMethod]
        public void OldNullDocTest() {
            Assert.AreEqual(originalDoc.CurrentRevision, Merger.Merge(originalDoc, null).CurrentRevision);
        }

        //Test for null documents and null texts
        [TestMethod]
        public void CurNullRevOldNullDocTest() {
            Assert.IsNull(Merger.Merge(nullRevDoc, null));
        }

        [TestMethod]
        public void OldNullRevCurNullDocTest() {
            Assert.IsNull(Merger.Merge(null, nullRevDoc));
        }

        [TestMethod]
        public void EmptyDocTest() {
            String emptyReference = "";
            Assert.AreEqual(emptyReference, Merger.Merge(emptyDoc, emptyDoc).CurrentRevision);
        }

        [TestMethod]
        public void InsertionDocTest() {
            Assert.AreEqual(insertionDoc.CurrentRevision, Merger.Merge(insertionDoc, originalDoc).CurrentRevision);
        }

        [TestMethod]
        public void AlterationDocTest() {
            Assert.AreEqual(alterationDoc.CurrentRevision, Merger.Merge(alterationDoc, originalDoc).CurrentRevision);
        }

        [TestMethod]
        public void SameDocTest() {
            Assert.AreEqual(originalDoc.CurrentRevision, Merger.Merge(originalDoc, originalDoc).CurrentRevision);
        }

        [TestMethod]
        public void TwoWaySplitDocTest() {
            Assert.AreEqual(twoWaySplitDocReference.CurrentRevision, Merger.Merge(twoWaySplitDocA, twoWaySplitDocB).CurrentRevision);
        }

        //Aaand here are the rest of the documents
        Document originalDoc = new Document {
            CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. "
        };

        Document insertionDoc = new Document {
            CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
I'VE BEEN WORKING ON THE RAILROAD, ALL THE LIFE-LONG DAYAYAYAY 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. "
        };

        Document alterationDoc = new Document {
            CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. gisM., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could kill the CATS. I feared to go very far 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. "
        };

        Document twoWaySplitDocA = new Document {
            CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
IMMAH FIRING MAH LAZOR
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. "
        };

        Document twoWaySplitDocB = new Document {
            CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train wars an raptors, raptors everywhere! Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
and depth, took us among the traditions of Turkish rule. "
        };

        Document twoWaySplitDocReference = new Document {
            CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
IMMAH FIRING MAH LAZOR
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. "
        };
    }
}
