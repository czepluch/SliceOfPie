using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SliceOfPie;

namespace SliceOfPieTests {
    [TestClass]
    public class MergerTest {
        Document nullDocA = new Document();
        Document nullDocB = new Document();

        Document emptyDocA = new Document() { CurrentRevision = "" };
        Document emptyDocB = new Document() { CurrentRevision = "" };

        //rest of the test documents declared in the bottom of file, because they are butt-ugly

        public MergerTest() {
        }

        [TestMethod]
        public void NullDocsTest() {
            Assert.IsNull(Merger.Merge(nullDocA, nullDocB));
        }

        [TestMethod]
        public void EmptyDocTest() {
            String emptyReference = "";
            Assert.AreEqual(emptyReference, Merger.Merge(emptyDocA, emptyDocB).CurrentRevision);
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
            Assert.AreEqual(twoWaySplitDocReference.CurrentRevision, Merger.Merge(twoWaySplitDocB, twoWaySplitDocA).CurrentRevision);
        }

        //Aaand here are the rest of te documents
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
6:46, but train wars an raptors, raptors everywhere! Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
and depth, took us among the traditions of Turkish rule. "
        };
    }
}
