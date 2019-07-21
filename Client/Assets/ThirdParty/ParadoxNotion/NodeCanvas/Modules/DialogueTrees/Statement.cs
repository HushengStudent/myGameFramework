using ParadoxNotion;
using NodeCanvas.Framework;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.DialogueTrees
{

    ///An interface to use for whats being said by a dialogue actor
    public interface IStatement
    {
        string text { get; }
        AudioClip audio { get; }
        string meta { get; }
    }

    ///Holds data of what's being said usualy by an actor
    [System.Serializable]
    public class Statement : IStatement
    {

        [SerializeField]
        private string _text = string.Empty;
        [SerializeField]
        private AudioClip _audio;
        [SerializeField]
        private string _meta = string.Empty;

        public string text {
            get { return _text; }
            set { _text = value; }
        }

        public AudioClip audio {
            get { return _audio; }
            set { _audio = value; }
        }

        public string meta {
            get { return _meta; }
            set { _meta = value; }
        }

        //required
        public Statement() { }
        public Statement(string text) {
            this.text = text;
        }

        public Statement(string text, AudioClip audio) {
            this.text = text;
            this.audio = audio;
        }

        public Statement(string text, AudioClip audio, string meta) {
            this.text = text;
            this.audio = audio;
            this.meta = meta;
        }

        ///Replace the text of the statement found in brackets, with blackboard variables ToString and returns a Statement copy
        public IStatement BlackboardReplace(IBlackboard bb) {
            var copy = ParadoxNotion.Serialization.JSONSerializer.Clone<Statement>(this);

            copy.text = copy.text.ReplaceWithin('[', ']', (input) =>
            {
                object o = null;
                if ( bb != null ) { //referenced blackboard replace
                    var v = bb.GetVariable(input, typeof(object));
                    if ( v != null ) {
                        o = v.value;
                    }
                }

                if ( input.Contains("/") ) { //global blackboard replace
                    var globalBB = GlobalBlackboard.Find(input.Split('/').First());
                    if ( globalBB != null ) {
                        var v = globalBB.GetVariable(input.Split('/').Last(), typeof(object));
                        if ( v != null ) {
                            o = v.value;
                        }
                    }
                }
                return o != null ? o.ToString() : input;
            });

            return copy;
        }

        public override string ToString() {
            return text;
        }
    }
}