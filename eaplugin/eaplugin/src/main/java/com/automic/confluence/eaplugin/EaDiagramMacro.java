package com.automic.confluence.eaplugin;

import java.util.Map;

import com.atlassian.renderer.RenderContext;
import com.atlassian.renderer.TokenType;
import com.atlassian.renderer.v2.RenderMode;
import com.atlassian.renderer.v2.macro.BaseMacro;
import com.atlassian.renderer.v2.macro.MacroException;
import com.atlassian.renderer.v2.macro.WysiwygBodyType;

public class EaDiagramMacro extends BaseMacro  {

	
	
	@Override
	public String execute(Map parameters, String body,
			RenderContext renderContext) throws MacroException {
		
		String server = System.getenv("EA_API_HOST");
		
		if(server == null || server.equals(""))
		{
			// return "The EA_API_HOST environment variable is not set correctly (e.g. myapiserver)";
			server = "ENTER_THE_DEFAULT_SERVER:3579";
		}
		
		String img = String.format("<img src='http://%s/api/v1/projects/%s/diagrams/%s/img' />",
				server,
				parameters.get("project"),
				parameters.get("guid"));
	
		return img;
	}

	@Override
	public RenderMode getBodyRenderMode() {
		return RenderMode.ALL;
	}

	@Override
	public boolean hasBody() {
		return false;
	}

	@Override
	public boolean isInline() {
		return true;
	}

	


}
